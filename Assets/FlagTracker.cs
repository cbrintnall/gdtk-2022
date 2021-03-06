using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Enables a gameobject if a flag has been set, or if not set (on start)
/// when an event fires of stating the flag has been set
/// </summary>
public class FlagTracker : MonoBehaviour
{
  [Tooltip("The flag you want to track")]
  public Flag Flag;
  [Tooltip("The minimum count to trigger an event")]
  public int Count = 1;
  [Tooltip("If the tracker should be removed after triggering (to prevent repeat triggers)")]
  public bool DestroyOnTrigger = true;
  [Tooltip("Optional event")]
  public UnityEvent OnFlag;
  [Tooltip("Fired when the flag is NOT true")]
  public UnityEvent OnFlagFailed;
  
  FlagManager flagManager;
  EventManager eventManager;

  private void Awake()
  {
    flagManager = FindObjectOfType<FlagManager>();
    eventManager = FindObjectOfType<EventManager>();

    CheckForFlag();

    eventManager.Register<FlagChangedEvent>(OnFlagChanged);
  }

  void OnFlagChanged(FlagChangedEvent ev)
  {
    if (ev.Flag == Flag)
    {
      CheckForFlag();
    }
  }

  void CheckForFlag()
  {
    if (flagManager.HasHappenedNTimes(Flag, Count))
    {
      Debug.Log($"Triggering {name}, flag {Flag} has happened at least {Count} times.");

      OnFlag?.Invoke();

      if (DestroyOnTrigger)
      {
        eventManager.Unregister<FlagChangedEvent>(OnFlagChanged);
        Destroy(this);
      }
    }
    else
    {
      OnFlagFailed?.Invoke();
    }
  }
}
