using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(Collider))]
public class TriggerEvent : MonoBehaviour
{
  [Tooltip("which gameobjects tags can trigger this")]
  public string[] AllowedTags;
  [Tooltip("if false, only fires an event once")]
  public bool AllowRepeating = true;
  public UnityEvent<Collider> OnTriggerEnteredCollider;
  public UnityEvent OnTriggerEntered;

  // Start is called before the first frame update
  void Start()
  {
    GetComponent<Collider>().isTrigger = true;

    if (AllowedTags.Length == 0)
    {
      Debug.LogError($"{name} TriggerEvent is has no allowed tags");
      enabled = false;
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (AllowedTags.Contains(other.tag))
    {
      OnTriggerEnteredCollider?.Invoke(other);
      OnTriggerEntered?.Invoke();
    }
  }
}
