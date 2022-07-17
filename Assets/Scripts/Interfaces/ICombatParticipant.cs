using UnityEngine;

public interface ICombatParticipant
{
  public GameObject Owner { get; }
  public HpPool Health { get; }

  void StartTurn();
}