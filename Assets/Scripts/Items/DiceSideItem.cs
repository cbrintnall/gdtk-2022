using System.Collections;
using UnityEngine;

public struct DiceLandedPayload
{
  public DiceController Controller;
  public EnemyController Target;
  public int Side;

  public ItemButton GetItemButton() => Controller.FaceItemsController.GetButtonForSide(Side);
}

public abstract class DiceSideItem : Pickupable
{
  public virtual Sprite ItemTexture => null;

  public override bool GiveToPlayer(PlayerController player)
  {
    player.GetComponent<PlayerDiceController>().GainSideItem(this);

    return true;
  }

  public override string ToString()
  {
    return $"name={Name}";
  }

  public abstract IEnumerator OnLanded(DiceLandedPayload payload);
}