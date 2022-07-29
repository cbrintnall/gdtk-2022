using System.Collections;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

[Singleton]
public class AudioManager : MonoBehaviour
{
  public static AudioManager Instance;
  public static Vector2 StandardRandomAmount => new Vector2(.9f, 1.1f);

  AudioSource player;

  private void Awake()
  {
    Instance = this;

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

  public void PlayRandomPitch(AudioClip clip, float volume) => PlayRandomPitch(clip, volume, StandardRandomAmount);
  public void PlayRandomPitch(AudioClip clip, float volume, Vector2 range) => Play(clip, volume, Random.Range(range.x, range.y));
}