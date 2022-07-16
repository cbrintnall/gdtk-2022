using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

[Singleton]
public class LevelManager : MonoBehaviour
{
  public Grid Grid;
  public PlayerController Player;
  public Level Level;

  private void Start()
  {
    // TODO: eventually this singleton will handle switching of levels, spawning of player, etc.
    Grid = FindObjectOfType<Grid>();
    Player = FindObjectOfType<PlayerController>();
    Level = FindObjectOfType<Level>();
  }
}
