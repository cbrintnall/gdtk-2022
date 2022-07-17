using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

public class MusicManager : MonoBehaviour
{
  public AudioSource MasterSource, SlaveSource1, SlaveSource2;
  public AudioClip[] MusicClips;
  public AudioClip[] FragmentClips;
  void Awake()
  {
  }

  private void Start()
  {
    Debug.Log(MusicClips[0]);
    PlayFragmentSong();
  }

  public void PlayAmbient(AudioClip clip)
  {
    Debug.Log("Should be playing music");
    MasterSource.clip = clip;
    MasterSource.loop = true;
    MasterSource.Play();
  }

  public void PlayFragmentSong()
  {
    MasterSource.clip = FragmentClips[0];
    MasterSource.loop = true;
    MasterSource.Play();

    SlaveSource1.clip = FragmentClips[2];
    SlaveSource1.loop = true;
    SlaveSource1.Play();
  }

  // Update is called once per frame
  void Update()
  {

  }
}
