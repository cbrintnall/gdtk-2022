using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public abstract class Pickupable : ScriptableObject 
{
  [InfoBox("$GainedItemText")]
  public string Name;
  [TextArea]
  public string Description;
  public AudioClip PickupSound;

  public static string ItemGainedText(string item) => $"You've gained \"{item}\"";

  private string GainedItemText() => $"Item gained text will look like: {ItemGainedText(Name)}";

  public abstract bool GiveToPlayer(PlayerController player);
}