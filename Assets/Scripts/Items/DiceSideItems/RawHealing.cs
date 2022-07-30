using DG.Tweening;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Side Items/RawHealing")]
public class RawHealing : DiceSideItem
{
  public Sprite Texture;
  public int Multiplier = 1;
  public AudioClip HealSound;

  public override Sprite ItemTexture => Texture;

  public override IEnumerator OnLanded(DiceLandedPayload payload)
  {
    FindObjectOfType<PlayerController>().Health.Heal(payload.Side * Multiplier);

    ItemButton sideButton = payload.Controller.FaceItemsController.GetButtonForSide(payload.Side);

    var tween = sideButton.transform.DOPunchScale(
      Vector3.one * 1.1f,
      .25f
    );

    AudioManager.Instance.PlayRandomPitch(HealSound, 1f, new Vector2(.9f, 1.1f));

    yield return new WaitUntil(() => !tween.IsActive() || tween.IsComplete());
  }
}