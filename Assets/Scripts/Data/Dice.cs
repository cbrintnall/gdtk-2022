using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dice", menuName = "Dice/Dice")]
public class Dice : Pickupable
{
  public Texture DiceTexture;
  public GameObject Prefab;
  public BaseItem[] DefaultItems;

  public override bool GiveToPlayer(PlayerController player)
  {
    player.GetComponent<PlayerDiceController>().GainDice(this);

    var eventManager = FindObjectOfType<EventManager>();
    var flagManager = FindObjectOfType<FlagManager>();

    eventManager.Publish(
      new DiceGainedEvent
      {
        DiceGained = this
      }
    );

    flagManager.Incr(Flag.DICE_GAINED);

    var ui = FindObjectOfType<PlayerUI>();

    ui.ShowText(
      new TextUIPayload()
      {
        TopText = ItemGainedText(this.Name),
        BottomText = Description,
        OpenSound = PickupSound
      }
    );

    return true;
  }
}