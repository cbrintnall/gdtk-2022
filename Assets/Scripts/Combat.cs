using Arc.Lib.Debug;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
  Queue<ICombatParticipant> turnOrder = new();

  DebugManager dbg;

  private void Awake()
  {
    dbg = FindObjectOfType<DebugManager>();
  }

  public void AddParticipant(ICombatParticipant participant)
  {
    turnOrder.Enqueue(participant);
  }

  public bool IsActiveParticipant(ICombatParticipant p)
  {
    Debug.LogError("Finish this fucking function");
    return true;
  }

  public void Begin() => DoNextTurn();

  private void DoNextTurn()
  {
    ICombatParticipant nextTurn = turnOrder.Dequeue();

    nextTurn.StartTurn();

    turnOrder.Enqueue(nextTurn);

    dbg.Track("Current turn", nextTurn.Owner.name);
  }
}