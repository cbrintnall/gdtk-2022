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

[RequireComponent(typeof(PlayerController))]
public class PlayerDiceController : MonoBehaviour
{
  // TODO: allow holding multiple dice, for now theres only one
  public DiceController HeldDie;
  private PlayerController playerController;

  private void Start()
  {
    playerController = GetComponent<PlayerController>();
  }

  public void GainDice(Dice dice)
  {
    // Create the new dice in the world under the overlay camera
    GameObject newDie = Instantiate(dice.Prefab, playerController.OverlayCamera.transform);

    DiceMovement dm = newDie.GetComponent<DiceMovement>();
    DiceController dc = newDie.GetComponent<DiceController>();

    Debug.Assert(dm != null);

    dc.AddItems(dice.DefaultItems);
    dm.Main = playerController.FirstPersonCamera;
    dm.Overlay = playerController.OverlayCamera;
    dm.ResetPosition();

    HeldDie = dc;
  }
}
