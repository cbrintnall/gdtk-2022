using System.Collections;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

[Singleton]
public class AudioManager : MonoBehaviour
{
  AudioSource player;

  private void Awake()
  {
    player = gameObject.AddComponent<AudioSource>();
    player.loop = false;
    player.playOnAwake = false;
  }

  public void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
  {
    player.PlayOneShot(clip);
    player.volume = volume;
    player.pitch = pitch;
  }
}