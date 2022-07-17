using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

public class MusicManager : MonoBehaviour
{
  public AudioSource MusicSource;
  public AudioClip[] MusicClips;
  void Awake()
  {
  }

  private void Start()
  {
    Debug.Log(MusicClips[0]);
    PlayAmbient(MusicClips[0]);
  }

  public void PlayAmbient(AudioClip clip)
  {
    Debug.Log("Should be playing music");
    MusicSource.clip = clip;
    MusicSource.loop = true;
    MusicSource.Play();
  }

  // Update is called once per frame
  void Update()
  {

  }
}
