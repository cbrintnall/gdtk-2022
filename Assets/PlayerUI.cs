using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class TextUIPayload
{
  public string TopText;
  public string BottomText;
  public AudioClip OpenSound;
}

public class PlayerUI : MonoBehaviour
{
  public GameObject TextPopupBackground;

  [Header("Dice text")]
  public GameObject Root;
  public TextMeshProUGUI ItemGainedText;
  public TextMeshProUGUI ItemGainedDescription;

  [Header("Audio")]
  public AudioSource player;

  private void Start()
  {
    var eventManager = FindObjectOfType<EventManager>();

    eventManager.Register<DiceGainedEvent>(NewDiceGained);
    eventManager.Register<PlayerMoveEvent>(_ => CloseUI());
  }

  void NewDiceGained(DiceGainedEvent ev) => ShowText(
    new TextUIPayload()
    {
      TopText = $"You've gained \"{ev.DiceGained.Name}\"",
      BottomText = ev.DiceGained.Description
    }
  );

  public void ShowText(TextUIPayload payload) 
  {
    TextPopupBackground.gameObject.SetActive(true);
    Root.gameObject.SetActive(true);
    ItemGainedText.text = payload.TopText;
    ItemGainedDescription.text = payload.BottomText;

    if (payload.OpenSound != null)
    {
      player.PlayOneShot(payload.OpenSound);
    }
  }

  private void CloseUI()
  {
    TextPopupBackground.gameObject.SetActive(false);
    Root.gameObject.SetActive(false);
  }

  private void Update()
  {
    if (TextPopupBackground.activeInHierarchy && Input.GetMouseButtonDown(0))
    {
      CloseUI();
    }
  }
}
