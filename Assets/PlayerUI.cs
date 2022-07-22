using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TextUIPayload
{
  public string TopText;
  public string BottomText;
  public AudioClip OpenSound;
  public Pickupable[] ItemOptions;
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
  const string NEED_ITEM_NAME = "NEED_ITEM_NAME";

  public GameObject TextPopupBackground;

  [Header("Dice text")]
  public GameObject Root;
  public TextMeshProUGUI ItemGainedText;
  public TextMeshProUGUI ItemGainedDescription;

  [Header("Options")]
  public GameObject OptionsRoot;
  public GameObject OptionButton;

  [Header("Combat")]
  public GameObject ParticipantsRoot;
  public CombatPanel PanelPrefab;

  [Header("Audio")]
  public AudioSource player;

  EventManager eventManager;
  TextUIPayload lastPayload;

  private void Start()
  {
    eventManager = FindObjectOfType<EventManager>();
    eventManager.Register<StartCombatEvent>(OnCombatBegin);
    eventManager.Register<CombatEndEvent>(OnCombatEnd);
  }

  public void ShowText(TextUIPayload payload) 
  {
    // Check if we're interrupting an existing dialogue, and if so we need to send off
    // a dialogue closed event
    if (TextPopupBackground.activeInHierarchy)
    {
      eventManager.Publish(
        new DialogueCloseEvent()
        {
          Payload = lastPayload
        }
      );
    }

    TextPopupBackground.gameObject.SetActive(true);
    Root.gameObject.SetActive(true);
    ItemGainedText.text = payload.TopText;
    ItemGainedDescription.text = payload.BottomText;

    if (payload.ItemOptions == null || payload.ItemOptions.Length == 0)
    {
      OptionsRoot.SetActive(false);
    }
    else
    {
      OptionsRoot.SetActive(true);

      foreach (var pickup in payload.ItemOptions)
      {
        GameObject option = Instantiate(OptionButton, OptionsRoot.transform);

        Button button = option.GetComponent<Button>();
        TextMeshProUGUI optionText = option.GetComponentInChildren<TextMeshProUGUI>();

        button.onClick.AddListener(() =>
        {
          // Somewhere downstream from this call it'll open another dialogue container,
          // so we don't want to call closeui here, otherwise we'll probably close that ui.
          pickup.GiveToPlayer(FindObjectOfType<PlayerController>());
        });

        optionText.text = string.IsNullOrEmpty(pickup.Name) ? NEED_ITEM_NAME : pickup.Name;
      }
    }

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

  void OnCombatEnd(CombatEndEvent ev)
  {
    for(int i = 0; i < ParticipantsRoot.transform.childCount; i++)
    {
      Destroy(ParticipantsRoot.transform.GetChild(i).gameObject);
    }
  }

  void OnCombatBegin(StartCombatEvent ev)
  {
    foreach(var participant in ev.ExistingParticipants)
    {
      CombatPanel panel = Instantiate(PanelPrefab, ParticipantsRoot.transform);

      panel.Participant = participant;
    }
  }

  private void CloseUI()
  {
    TextPopupBackground.gameObject.SetActive(false);
    Root.gameObject.SetActive(false);

    // remove all existing options
    foreach(Transform child in OptionsRoot.transform)
    {
      Destroy(child.gameObject);
    }

    eventManager.Publish(new DialogueCloseEvent() { Payload = lastPayload });
  }

  private void Update()
  {
    if (TextPopupBackground.activeInHierarchy && !OptionsRoot.activeInHierarchy && Input.GetMouseButtonDown(0))
    {
      CloseUI();
    }
  }
}
