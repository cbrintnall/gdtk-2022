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

public class DialogueCloseEvent : BaseEvent 
{
  public TextUIPayload Payload;
}

public class DialogueOpenEvent : BaseEvent
{
  public TextUIPayload Payload;
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

  EventManager eventManager;
  TextUIPayload lastPayload;

  private void Start()
  {
    eventManager = FindObjectOfType<EventManager>();
  }

  public void ShowText(TextUIPayload payload) 
  {
    TextPopupBackground.gameObject.SetActive(true);
    Root.gameObject.SetActive(true);
    ItemGainedText.text = payload.TopText;
    ItemGainedDescription.text = payload.BottomText;

    eventManager.Publish(
      new DialogueOpenEvent()
      {
        Payload = payload
      }
    );

    lastPayload = payload;

    if (payload.OpenSound != null)
    {
      player.PlayOneShot(payload.OpenSound);
    }
  }

  private void CloseUI()
  {
    TextPopupBackground.gameObject.SetActive(false);
    Root.gameObject.SetActive(false);

    eventManager.Publish(new DialogueCloseEvent() { Payload = lastPayload });
  }

  private void Update()
  {
    if (TextPopupBackground.activeInHierarchy && Input.GetMouseButtonDown(0))
    {
      CloseUI();
    }
  }
}
