using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class DiceMovement : MonoBehaviour
{
  public Camera Overlay;
  public CinemachineVirtualCamera Main;

  // TODO: These should be cached on a settings changed event or something
  public int ScreenWidth;
  public int ScreenHeight;

  public Vector3 Anchor;

  // If the dice is attached to the player or being moved into world space
  bool attachedToPlayer = true;

  private void Awake()
  {
    attachedToPlayer = true;
  }

  private void Update()
  {
    if (attachedToPlayer)
    {
      transform.position = Overlay.ViewportToWorldPoint(Anchor);
    }
  }
}
