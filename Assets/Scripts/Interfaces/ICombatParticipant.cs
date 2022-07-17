using UnityEngine;

public interface ICombatParticipant
{
  public GameObject Owner { get; }

  void StartTurn();
}