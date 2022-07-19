using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

public class GameStateChangedEvent : BaseEvent
{
  public GameState NewState;
  public GameState OldState;
}

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
  public Combat CurrentCombat;
  public GameState GameState
  {
    get => _gameState;
    set
    {
      GameState old = _gameState;
      _gameState = value;

      eventManager.Publish(
        new GameStateChangedEvent()
        {
          OldState = old,
          NewState = value
        }
      );
    }
  }

  private EventManager eventManager;
  private GameState _gameState = GameState.EXPLORING;

  private void Start()
  {
    // TODO: eventually this singleton will handle switching of levels, spawning of player, etc.
    eventManager = FindObjectOfType<EventManager>();
    Player = FindObjectOfType<PlayerController>();
    Level = FindObjectOfType<Level>();
    GameState = GameState.EXPLORING;

    eventManager.Register<StartCombatEvent>(SetupCombat);
    eventManager.Register<CombatEndEvent>(EndCombat);
  }

  void EndCombat(CombatEndEvent ev)
  {
    // combat game object handles destroying itself 
    GameState = GameState.EXPLORING;
  }

  void SetupCombat(StartCombatEvent ev)
  {
    GameState = GameState.IN_COMBAT;

    GameObject combatParent = new GameObject("COMBAT_PARENT");
    combatParent.transform.SetParent(transform);
    CurrentCombat = combatParent.AddComponent<Combat>();

    foreach(var startingParticipant in ev.ExistingParticipants)
    {
      CurrentCombat.AddParticipant(startingParticipant);
    }

    CurrentCombat.Begin();
  }
}
