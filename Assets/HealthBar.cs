using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HealthBar : MonoBehaviour
{
  public HpPool Health;
  public float LerpSpeed = 2f;
  public RawImage Backdrop;
  public RawImage HealthImg;
  public TMPro.TextMeshProUGUI HealthText;

  private void Update()
  {
    HealthImg.rectTransform.anchorMax = new Vector2(
      Mathf.Lerp(HealthImg.rectTransform.anchorMax.x, Health.NormalizedHP, Time.deltaTime * LerpSpeed),
      1f
    );

    HealthText.text = $"{Health.CurrentHp}/{Health.MaxHp}";
  }
}
