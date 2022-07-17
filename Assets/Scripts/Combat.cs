using Arc.Lib.Debug;
using System.Collections.Generic;
using UnityEngine;

public class CombatEndEvent : BaseEvent
{ }

public class Combat : MonoBehaviour
{
  const int MaxParticipants = 3;
  public int ParticipantCount => turnOrder.Count;

  Queue<ICombatParticipant> turnOrder = new();

  ICombatParticipant center, left, right;

  DebugManager dbg;

  private void Awake()
  {
    dbg = FindObjectOfType<DebugManager>();

    var evt = FindObjectOfType<EventManager>();
    evt.Register<DeathEvent>(OnDeath);

    Debug.Log("New combat created");
  }

  void EndCombat()
  {
    FindObjectOfType<EventManager>().Publish(
      new CombatEndEvent() { }
    );
  }

  void OnDeath(DeathEvent ev)
  {
    if (IsActiveParticipant(ev.target.GetComponent<ICombatParticipant>()))
    {
      EndCombat();
    }
  }

  public bool AddParticipant(ICombatParticipant participant)
  {
    if (ParticipantCount > MaxParticipants-1)
    {
      Debug.LogWarning($"Rejecting combat participant {participant.Owner.name}, too many already involved");
      return false;
    }

    //if (!participant.Owner.CompareTag("Player"))
    //{
    //  Vector2Int offset = Vector2Int.zero;

    //  if (center == null)
    //  {
    //    center = participant;
    //  }
    //  else if (left == null)
    //  {
    //    left = participant;
    //    offset = Vector2Int.left;
    //  }
    //  else if (right == null)
    //  {
    //    right = participant;
    //    offset = Vector2Int.right;
    //  }

    //  Grid grid = participant.Owner.GetComponent<GridMovement>().Grid;
    //  Vector2Int target = center.Owner.GetComponent<GridMovement>().CurrentTile + offset;
    //  Vector3 worldPosition = grid.CellToWorld(new Vector3Int(target.x, 0, target.y));

    //  participant.Owner.transform.position = new Vector3(worldPosition.x, transform.position.y, worldPosition.y);
    //  Utils.AlignToGrid(grid, participant.Owner.transform);
    //}

    turnOrder.Enqueue(participant);

    Debug.Log($"{participant.Owner.name} is joining combat encounter.");
    return true;
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

  void OnDeath(ICombatParticipant participant)
  {

  }
}