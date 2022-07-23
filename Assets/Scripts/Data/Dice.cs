using Sirenix.OdinInspector;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "New Dice", menuName = "Dice/Dice")]
public class Dice : Pickupable
{
  [Tooltip("The actual dice, should have a DiceController and DiceMovement component")]
  public GameObject Prefab;
  [Tooltip("The items this die begins with")]
  public BaseItem[] DefaultItems;
  [Tooltip("How many dice this side has, influences how large the generated data arrays can be")]
  public int NumberOfSides;

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