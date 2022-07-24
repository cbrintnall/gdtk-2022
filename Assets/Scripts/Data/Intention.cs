using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new intention", menuName = "Intention")]
public class Intention : ScriptableObject
{
  public Sprite Sprite;
  public AudioClip[] OnActive;
}