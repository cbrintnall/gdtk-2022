using Arc.Lib.Utils;
using System.Collections;
using UnityEngine;

public class WaitForEvent<T> : IEnumerator where T : BaseEvent
{
  public object Current => null;

  bool eventHappened;

  WaitForEvent()
  {
    SingletonLoader.Get<EventManager>().Register<T>(_ => eventHappened = true);
  }

  public bool MoveNext() => eventHappened;

  public void Reset()
  {
    eventHappened = false;
  }
}