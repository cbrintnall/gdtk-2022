using Arc.Lib.Debug;
using UnityEngine;

public class PlayerMoveEvent : BaseEvent
{
  public Vector2Int NewTile;
}

[RequireComponent(typeof(GridMovement))]
public class PlayerController : MonoBehaviour
{
  GridMovement GridMover;
  DebugManager dbg;
  EventManager eventManager;
  LevelManager levelManager;

  private void Start()
  {
    GridMover = GetComponent<GridMovement>();
    dbg = FindObjectOfType<DebugManager>();
    eventManager = FindObjectOfType<EventManager>();
    levelManager = FindObjectOfType<LevelManager>();
    eventManager.Register<StartCombatEvent>(OnStartCombat);
  }

  public void NotifyPlayerMoved() => eventManager.Publish(new PlayerMoveEvent { NewTile = Utils.xy(GridMover.CurrentTile) });

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Tilde))
    {
      dbg.enabled = !dbg.enabled;
    }

    if (Input.GetKeyDown(KeyCode.D))
    {
      GridMover.Rotate(true);
    }
    if (Input.GetKeyDown(KeyCode.A))
    {
      GridMover.Rotate(false);
    }

    if (Input.GetKeyDown(KeyCode.W) && levelManager.GameState == GameState.EXPLORING)
    {
      Vector2Int targetTile = GridMover.GetTargetTile();
      var enemies = FindObjectsOfType<EnemyController>();
      EnemyController collidedWith = null;
      foreach (EnemyController enemy in enemies)
      {
        var enemyMover = enemy.GetComponent<GridMovement>();
        Vector2Int enemyPos = Utils.xy(enemyMover.CurrentTile);
        if (enemyPos == targetTile)
        {
          collidedWith = enemy;
          break;
        }
      }

      if (collidedWith == null)
      {
        GridMover.Move(1);
      }
      else
      {
        eventManager.Publish(new StartCombatEvent { 
          EnemyName = collidedWith.name,
          EnemyPosition = Utils.xy(collidedWith.GetComponent<GridMovement>().CurrentTile)});
      }
    }

    dbg.Track("Player Tile", GridMover.CurrentTile);
  }

  public void OnStartCombat(StartCombatEvent e)
  {
    Debug.Log("Player is in combat state now");
    levelManager.GameState = GameState.IN_COMBAT;
  }
}
