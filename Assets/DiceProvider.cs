using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DiceProvider : MonoBehaviour
{
  public Dice ProvidedDice;

  private void Awake()
  {
    GetComponent<Collider>().isTrigger = true;

    // TODO: check if player already has dice, if so just destroy self
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      var controller = other.GetComponent<PlayerDiceController>();
      controller.GainDice(ProvidedDice);

      // intentionally just destroy the component, if this is attached to
      // a chest or something we don't want to destroy that
      Destroy(this);
    }
  }
}
