using System.Collections;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

[Singleton]
public class JobsManager : MonoBehaviour
{
  public void Coroutine(IEnumerator routine) => StartCoroutine(routine);
}