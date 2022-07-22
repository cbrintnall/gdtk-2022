using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem : Pickupable
{
  public AudioClip RejectedSound;

  public abstract int getPriority();
  public abstract bool HandleAssignToDice(DiceController dc);
  public virtual void updateTargetState(TargetState state) { }
  public virtual void updateProbState(ProbState state) { }
  public virtual void updateAttackState(AttackState state) { }

  public override bool GiveToPlayer(PlayerController player)
  {
    var dc = player.GetComponent<PlayerDiceController>();
    var ui = FindObjectOfType<PlayerUI>();

    if (dc.HeldDie == null)
    {
      ui.ShowText(
        new TextUIPayload()
        {
          TopText = "You are not powerful enough to equip this..",
          BottomText = "This item appears to have an infinite weight, as you cannot even make it budge. Perhaps something else around here can help you?",
          OpenSound = RejectedSound
        }
      );

      return false;
    }

    if (HandleAssignToDice(dc.HeldDie))
    {
      var flagManager = FindObjectOfType<FlagManager>();

      flagManager.Incr(Flag.ITEM_GAINED);

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
    else
    {
      // If we couldn't assign the die.. TODO:
      // play sound
      // notify player
      // dont allow wherever it came from to disable, they should be able to try and pick it up again
    }

    return false;
  }
}
