using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Pretty much just a semaphore, but for checking conditions. IE if
/// multiple things are saying "dont move" to the player, we dont want
/// that state to be changed if only one says "you can move now"
/// </summary>
[Serializable]
public class StackingBool
{
  private protected int Count = 0;

  public StackingBool(int initialCount = 0)
  {
    Count = initialCount;
  }

  public static implicit operator bool(StackingBool sb) => sb.Count == 0;

  public void Incr() => Count = Mathf.Max(Count + 1, 0);
  public void Decr() => Count = Mathf.Max(Count - 1, 0);
}