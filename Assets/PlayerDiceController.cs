using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TargetState
{
    public List<Vector2Int> relativeTargets = new List<Vector2Int>();
}

public class ProbState
{
    public List<double> probs = new List<double>();
}

public class AttackState
{
    public int rollResult;
    public int damage;
}

public class DiceGainedEvent : BaseEvent
{
  public Dice DiceGained;
}

public class PlayerDiceController : MonoBehaviour
{
  public List<Dice> HeldDice;
  public AudioClip PickupAudio;

  private EventManager eventManager;
  private FlagManager flagManager;

  private void Start()
  {
    eventManager = FindObjectOfType<EventManager>();
    flagManager = FindObjectOfType<FlagManager>();
  }

  public void GainDice(Dice dice)
  {
    HeldDice.Add(dice);

    eventManager.Publish(
      new DiceGainedEvent
      {
        DiceGained = dice
      }
    );

    var ui = FindObjectOfType<PlayerUI>();

    ui.ShowText(
      new TextUIPayload()
      {
        TopText = $"You've gained \"{dice.Name}\"",
        BottomText = dice.Description,
        OpenSound = PickupAudio
      }
    );
  }
}
