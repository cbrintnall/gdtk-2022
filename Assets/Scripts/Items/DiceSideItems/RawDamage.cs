using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// A dice side item that does a certain amount of damage,
/// has a multiplier that takes in the side we landed on.
/// </summary>
[CreateAssetMenu(menuName = "Side Items/RawDamage")]
public class RawDamage : DiceSideItem
{
  public Sprite Texture;
  public int Multiplier = 1;
  public AudioClip AttackSound;

  public override Sprite ItemTexture => Texture;

  public override void OnLanded(DiceLandedPayload payload)
  {
    payload.Target.Health.Damage(payload.Side * Multiplier);
    payload.GetItemButton().transform.DOPunchScale(
      Vector3.one * 1.1f,
      .25f
    );

    AudioManager.Instance.PlayRandomPitch(AttackSound, 1f, new Vector2(.9f, 1.1f));
  }
}