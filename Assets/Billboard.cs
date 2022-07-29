using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
  public bool Reverse;

  Camera mainCamera;

  private void Start()
  {
    mainCamera = Camera.main;
  }

  private void Update()
  {
    transform.LookAt(mainCamera.transform);
    transform.rotation = Quaternion.Euler(
      0f,
      transform.rotation.eulerAngles.y,
      transform.rotation.eulerAngles.z
    );

    if (Reverse)
      transform.forward = -transform.forward;
  }
}
