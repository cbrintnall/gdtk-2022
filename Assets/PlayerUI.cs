using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
  public GameObject TextPopupBackground;

  [Header("Dice text")]
  public GameObject Root;
  public TextMeshProUGUI ItemGainedText;
  public TextMeshProUGUI ItemGainedDescription;

  private void Start()
  {
    var eventManager = FindObjectOfType<EventManager>();

    eventManager.Register<DiceGainedEvent>(NewDiceGained);
  }

  public void NewDiceGained(DiceGainedEvent ev)
  {
    TextPopupBackground.gameObject.SetActive(true);
    Root.gameObject.SetActive(true);
    ItemGainedText.text = $"You've gained {ev.DiceGained.Name}";
    ItemGainedDescription.text = ev.DiceGained.Description;
  }

  private void Update()
  {
    if (TextPopupBackground.activeInHierarchy && Input.GetMouseButtonDown(0))
    {
      TextPopupBackground.gameObject.SetActive(false);
      Root.gameObject.SetActive(false);
    }
  }
}
