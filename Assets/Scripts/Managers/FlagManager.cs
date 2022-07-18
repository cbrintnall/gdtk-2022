using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

public class FlagChangedEvent : BaseEvent
{
  public Flag Flag;
  public int Count;
}

/// <summary>
/// A class for storing game conditions.. IE player picked up X dice,
/// player has progressed to y stage, player has picked up z key
/// </summary>
[Singleton]
public class FlagManager : MonoBehaviour 
{
  Dictionary<Flag, int> flags = new();

  EventManager eventManager;

  private void Start()
  {
    eventManager = FindObjectOfType<EventManager>();
  }

  public void Incr(Flag flag, int amt = 1)
  {
    if (!flags.ContainsKey(flag))
      flags[flag] = 0;

    flags[flag] += amt;

    eventManager.Publish(
      new FlagChangedEvent()
      {
        Flag = flag,
        Count = flags[flag]
      }
    );
  }

  public bool HasHappenedAtLeastOnce(Flag flag) => HasHappenedNTimes(flag, 1);
  public bool HasHappenedNTimes(Flag flag, int n) => flags.TryGetValue(flag, out int val) && val >= n;
}
