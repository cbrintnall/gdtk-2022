using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

public enum GameState
{
  EXPLORING,
  IN_COMBAT
}

[Singleton]
public class LevelManager : MonoBehaviour
{
  public PlayerController Player;
  public Level Level;
  public GameState GameState;

  private void Start()
  {
    // TODO: eventually this singleton will handle switching of levels, spawning of player, etc.
    Player = FindObjectOfType<PlayerController>();
    Level = FindObjectOfType<Level>();
    GameState = GameState.EXPLORING;
  }
}
