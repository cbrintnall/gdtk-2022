using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
  public TMPro.TextMeshProUGUI Title;
  public TMPro.TextMeshProUGUI Paragraph;

  RectTransform rectTransform;

  private void Start()
  {
    rectTransform = (RectTransform)transform;
    rectTransform.localPosition = Vector3.up * rectTransform.sizeDelta.y;
  }

  private void Update()
  {
    rectTransform.localPosition = Vector3.up * rectTransform.sizeDelta.y;
  }
}
