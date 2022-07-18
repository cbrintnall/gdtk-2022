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

  FlagManager flagManager;
  EventManager eventManager;

  private void Start()
  {
    flagManager = FindObjectOfType<FlagManager>();
    eventManager = FindObjectOfType<EventManager>();

    SyncObjectStateToFlag();

    if (!gameObject.activeInHierarchy)
    {
      eventManager.Register<FlagChangedEvent>(OnFlagChanged);
    }
  }

  void OnFlagChanged(FlagChangedEvent ev)
  {
    if (ev.Flag == Flag)
    {
      SyncObjectStateToFlag();
    }
  }

  void SyncObjectStateToFlag()
  {
    bool isHappening = flagManager.HasHappenedNTimes(Flag, Count);

    gameObject.SetActive(isHappening);

    if (isHappening)
    {
      Debug.Log($"Enabling {name}, flag {Flag} has happened at least {Count} times.");

      OnFlag?.Invoke();

      if (DestroyOnTrigger)
      {
        eventManager.Unregister<FlagChangedEvent>(OnFlagChanged);
        Destroy(this);
      }
    }
  }
}
