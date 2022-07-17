using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ConstantRotation : MonoBehaviour
{
  public float SpinSpeed = .4f;

  Tweener spinTween;

  private void Awake()
  {
    spinTween = transform
      .DORotate(Vector3.up * 360f, SpinSpeed, RotateMode.LocalAxisAdd)
      .SetEase(Ease.InOutCubic)
      .SetRecyclable(true)
      .SetAutoKill(false)
      .SetLoops(-1);
  }

  private void OnEnable()
  {
    spinTween.Restart();
  }

  private void OnDisable()
  {
    spinTween.Pause();
  }
}
