using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
  public TextUIPayload Text;

  public void TriggerDialogue() => FindObjectOfType<PlayerUI>().ShowText(Text); 
}
