using UnityEngine;

public interface ICombatParticipant
{
  public GameObject Owner { get; }
  public HpPool Health { get; }
  public string Name => Owner.name;

  void StartTurn();
}