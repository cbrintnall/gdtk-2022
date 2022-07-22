using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomNoise : MonoBehaviour
{
  [Header("Clips")]
  public AudioClip[] Audio;

  [MinMaxSlider(1f, 300f, true)]
  public Vector2 MinMaxTimeSeconds;

  AudioSource player;

  private void Awake()
  {
    GetComponents();
  }

  IEnumerator Start()
  {
    while (enabled)
    {
      AudioClip chosenClip = Audio.Random();

      yield return new WaitForSeconds(Random.Range(MinMaxTimeSeconds.x, MinMaxTimeSeconds.y) + chosenClip.length);

      player.PlayOneShot(chosenClip);
    }
  }

  private void OnValidate()
  {
    GetComponents();
  }

  void GetComponents()
  {
    player = GetComponent<AudioSource>();
  }
}
