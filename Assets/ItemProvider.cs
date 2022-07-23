using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ItemProvider : MonoBehaviour
{
  public UnityEvent PlayerPickedUp;
  public Pickupable[] Pickups;

  TextUIPayload currentPayload;
  PlayerUI playerui;

  private void Awake()
  {
    GetComponent<Collider>().isTrigger = true;
    // TODO: check if player already has item, if so just destroy self
  }

  private void Start()
  {
    playerui = FindObjectOfType<PlayerUI>();
    FindObjectOfType<EventManager>().Register<DialogueCloseEvent>(OnDialogueClosed);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
    {
      if (currentPayload != null) return;

      int diceCount = Pickups.Where(pickup => pickup is Dice).Count();

      // If the player is not holding a dice, and this chest isn't entirely composed of dice options,
      // we cannot let the player open it yet, as the player has no die to equip items to
      if (player.GetComponent<PlayerDiceController>().HeldDie == null && diceCount != Pickups.Length)
      {
        playerui.ShowText(
          new TextUIPayload()
          {
            TopText = "No matter how hard you try, the chest won't open..",
            BottomText = "Something appears to be forcing this chest to stay closed.. Perhaps you are missing a key of sorts."
          }
        );

        return;
      }

      if (Pickups.Length > 1)
      {
        playerui.ShowOptions(GetPickupOptions(), OnPickupChosen);
      }
      else if (Pickups.Length == 1)
      {
        if (Pickups[0].GiveToPlayer(player))
        {
          FinalizePickup();
        }
      }
    }
  }

  void OnPickupChosen(Pickupable pickup) => pickup.GiveToPlayer(FindObjectOfType<PlayerController>());

  TextUIChoicesPayload<Pickupable> GetPickupOptions() => new TextUIChoicesPayload<Pickupable>()
  {
    TopText = "The chest contains multiple treasures..",
    BottomText = "But attempting to take multiple burns your hands, you must only choose one.",
    Options = Pickups.Select(
      pickup => new OptionsPayload<Pickupable>()
      {
        Text = pickup.Name,
        Option = pickup
      }
    ).ToArray()
  };

  void FinalizePickup()
  {
    PlayerPickedUp?.Invoke();
    // intentionally just destroy the component, if this is attached to
    // a chest or something we don't want to destroy that
    Destroy(this);
  }

  private void OnDialogueClosed(DialogueCloseEvent ev)
  {
    if (ev.Payload == currentPayload)
    {
      Debug.Log($"{nameof(ItemProvider)}: Closed dialogue was our options, cleaning self up..");

      PlayerPickedUp?.Invoke();
      // intentionally just destroy the component, if this is attached to
      // a chest or something we don't want to destroy that
      Destroy(this);
    }
  }
}
