using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Button))]
[ExecuteInEditMode]
public class Toggleable : MonoBehaviour
{
  [Header("Audio")]
  public AudioClip ToggleOn;
  public AudioClip ToggleOff;

  [Header("Scaling")]
  public float TargetScale = 1.1f;
  public float ScaleTimeSeconds = .25f;

  [Header("Toggling")]
  public UnityEvent<bool> Changed;
  public bool Toggled
  {
    get => _toggled;
    set
    {
      var targetValue = value ? TargetScale * Vector3.one : Vector3.one;

      if (existingScaleTween != null && existingScaleTween.IsActive())
      {
        existingScaleTween.Kill(true);
      }

      existingScaleTween = DOTween.To(
        () => _graphic.rectTransform.localScale,
        scale => _graphic.rectTransform.localScale = scale,
        targetValue,
        ScaleTimeSeconds
      );

      _toggled = value;
      Changed?.Invoke(value);

      audioManager.PlayRandomPitch(value ? ToggleOn : ToggleOff, 1f, new Vector2(.9f, 1.1f));
    }
  }

  Tweener existingScaleTween;
  [ReadOnly]
  [ShowInInspector]
  bool _toggled;
  Graphic _graphic;
  AudioManager audioManager;

  private void Start()
  {
    _graphic = GetComponent<Graphic>();
    var btn = GetComponent<Button>();
    btn.onClick.AddListener(Toggle);

    audioManager = FindObjectOfType<AudioManager>();
  }

  [Button]
  public void Toggle() => Toggled = !Toggled;
}
