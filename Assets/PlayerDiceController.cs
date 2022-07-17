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

  private EventManager eventManager;

  private void Start()
  {
    eventManager = FindObjectOfType<EventManager>();
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
  }
}
