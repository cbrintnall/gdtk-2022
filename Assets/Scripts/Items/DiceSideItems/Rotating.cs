using Arc.Lib.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Functionality changes after the dice lands, rotating through
/// a series of other dice sides.
/// 
/// IE one turn attack, next turn heals, then back to attack. You can even
/// embed Rotating die within eachother, although I don't recommend that.
/// </summary>
[CreateAssetMenu]
public class Rotating : DiceSideItem
{
  public DiceSideItem[] Rotation;
  public AudioClip SwitchItemSound;

  int currentSide = 0;

  public override Sprite ItemTexture => Rotation[currentSide].ItemTexture;

  public override IEnumerator OnLanded(DiceLandedPayload payload)
  {
    yield return Rotation[currentSide].OnLanded(payload);

    int lastSide = currentSide;
    currentSide = (currentSide + 1) % Rotation.Length;

    SingletonLoader.Get<AudioManager>().PlayRandomPitch(SwitchItemSound, 1f, new Vector2(.9f, 1.1f));

    ItemButton btn = payload.GetItemButton();
    var tween = btn.transform.DOShakeRotation(.25f);
    btn.Item = Rotation[currentSide];

    Debug.Log($"payload_side={payload.Side},side={btn.Side},last={lastSide},current={currentSide}");

    yield return new WaitUntil(() => !tween.IsActive() || tween.IsComplete());
  }
}
