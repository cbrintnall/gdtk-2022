using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

public abstract class BaseEvent { }

[Singleton]
public class EventManager : MonoBehaviour
{
  Dictionary<string, List<Delegate>> callbacks = new();

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

    foreach(var cb in callbacks[key])
    {
      if (cb is Action<T> callbackAction)
      {
        callbackAction.Invoke(evnt);
      }
    }
  }
}