using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagSetter : MonoBehaviour
{
  public Flag Flag;
  public int Count = 1;

  public void Set() => FindObjectOfType<FlagManager>().Incr(Flag, Count);
}
