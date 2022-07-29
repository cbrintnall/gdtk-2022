using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[Serializable]
public class TextUIPayload
{
  public string TopText;
  public string BottomText;
  public AudioClip OpenSound;
}

public class ItemSwapChosenEvent : BaseEvent
{
  //"Chosen" determines what item is being swapped.
  public DiceSideItem Chosen;
  //FromDice determines if it's from the swap bar, or on the dice
  public bool FromDice;
  //DesiredSide is the side we want to swap for, if FromDice=true, this doesn't matter
  public int DesiredSide;

  public bool IsValidSwap(ItemSwapChosenEvent other)
  {
    return Chosen != other.Chosen && FromDice != other.FromDice && DesiredSide != other.DesiredSide;
  }
}

public class TextUIChoicesPayload<T> : TextUIPayload
{
  public OptionsPayload<T>[] Options;
}

public class OptionsPayload<T>
{
  public T Option;
  public string Text;
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

  [Header("Options")]
  public GameObject OptionsRoot;
  public GameObject OptionButton;

  [Header("Combat")]
  public GameObject ParticipantsRoot;
  public CombatPanel PanelPrefab;
  public RectTransform CombatItemOptionsParent;
  public RectTransform CombatItemOptionButton;

  [Header("Audio")]
  public AudioSource player;

  EventManager eventManager;
  TextUIPayload lastPayload;
  Tweener existingSideItemsTween;

  private void Start()
  {
    eventManager = FindObjectOfType<EventManager>();
    eventManager.Register<StartCombatEvent>(OnCombatBegin);
    eventManager.Register<CombatEndEvent>(OnCombatEnd);
  }

  [Button("Text Options")]
  private void TestOptions()
  {
    Action<string> onOptionChosen = (opt) =>
    {
      ShowText(
        new TextUIPayload()
        {
          TopText = "Thanks for choosing an option!",
          BottomText = $"You chose: {opt}"
        }
      );
    };

    string[] options = new string[]
    {
      "Option #1",
      "Option #2",
      "Option #3"
    };

    ShowOptions(
      new TextUIChoicesPayload<string>()
      {
        TopText = "Hello",
        BottomText = "Testing!",
        Options = options.Select(
          opt => new OptionsPayload<string>()
          {
            Text = opt,
            Option = opt
          }
        ).ToArray()
      },
      onOptionChosen
    );
  }

  public void ShowOptions<T>(TextUIChoicesPayload<T> payload, Action<T> onOptionChosen)
  {
    ShowText(payload);

    OptionsRoot.SetActive(true);

    StartCoroutine(OptionsCoroutine(payload.Options, onOptionChosen));
  }

  IEnumerator OptionsCoroutine<T>(OptionsPayload<T>[] options, Action<T> onOptionChosen)
  {
    bool optionChosen = false;

    foreach (var item in options)
    {
      GameObject option = Instantiate(OptionButton, OptionsRoot.transform);

      Button button = option.GetComponent<Button>();
      TextMeshProUGUI optionTextLabel = option.GetComponentInChildren<TextMeshProUGUI>();

      button.onClick.AddListener(() =>
      {
        onOptionChosen(item.Option);
        optionChosen = true;
      });

      optionTextLabel.text = item.Text;
    }

    yield return new WaitUntil(() => optionChosen);
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
    OptionsRoot.gameObject.SetActive(false);
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

  public void SetDiceItemOptions(List<DiceSideItem> options)
  {
    if (!CombatItemOptionsParent.gameObject.activeInHierarchy) return;

    foreach(RectTransform childTransform in CombatItemOptionsParent.transform)
    {
      Destroy(childTransform.gameObject);
    }

    foreach(var option in options)
    {
      var nextButton = Instantiate(CombatItemOptionButton, CombatItemOptionsParent.transform);

      nextButton.anchoredPosition = Vector3.zero;
      nextButton.GetComponent<ItemButton>().Item = option;
    }
  }

  public void HideItemOptions() => MoveItemOptions(1.2f, true);
  public void ShowItemOptions() => MoveItemOptions(0f, false);
  public void ToggleItemOptions() 
  {
    if (CombatItemOptionsParent.gameObject.activeInHierarchy)
    {
      HideItemOptions();
    }
    else
    {
      ShowItemOptions();
    }
  }

  void MoveItemOptions(float anchorY, bool after)
  {
    CombatItemOptionsParent.gameObject.SetActive(!after);
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
