using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPanel : MonoBehaviour
{
  public HealthBar HealthBar;
  public TMPro.TextMeshProUGUI Name;

  public ICombatParticipant Participant
  {
    get => _participant;
    set
    {
      _participant = value;

      HealthBar.Health = value.Health;
      Name.text = value.Name;
    }
  }

  private ICombatParticipant _participant;
}
