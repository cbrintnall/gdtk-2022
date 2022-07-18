using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
  public TextUIPayload Text;
  public UnityEvent OnClosed;

  private void Start()
  {
    var evtManager = FindObjectOfType<EventManager>();

    evtManager.Register<DialogueCloseEvent>(OnDialogueClosed);
  }

  void OnDialogueClosed(DialogueCloseEvent ev)
  {
    if (ev.Payload == Text)
    {
      OnClosed?.Invoke();
    }
  }

  public void TriggerDialogue() => FindObjectOfType<PlayerUI>().ShowText(Text); 
}
