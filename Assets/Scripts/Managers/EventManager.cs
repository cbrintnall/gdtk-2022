using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Arc.Lib.Utils.SingletonLoader;

public abstract class BaseEvent { }

[Singleton]
public class EventManager : MonoBehaviour
{
  Dictionary<string, List<Delegate>> callbacks = new();

  public void Unregister<T>(Action<T> cb) where T : BaseEvent
  {
    string key = typeof(T).ToString();

    if (!callbacks.ContainsKey(key))
    {
      callbacks[key] = new List<Delegate>();
    }

    callbacks[key].Remove(cb);
  }

  public void Register<T>(Action<T> cb) where T : BaseEvent
  {
    string key = typeof(T).ToString();

    if (!callbacks.ContainsKey(key))
    {
      callbacks[key] = new List<Delegate>();
    }

    callbacks[key].Add(cb);
  }

  public void Publish<T>(T evnt) where T : BaseEvent
  {
    string key = typeof(T).ToString();

    if (!callbacks.ContainsKey(key))
    {
      callbacks[key] = new List<Delegate>();
    }

    foreach(var cb in callbacks[key].GetRange(0, callbacks[key].Count))
    {
      if (cb is Action<T> callbackAction)
      {
        callbackAction.Invoke(evnt);
      }
    }
  }
}