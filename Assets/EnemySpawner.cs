using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlagTracker))]
public class EnemySpawner : MonoBehaviour
{
  [Tooltip("The enemy to spawn")]
  public GameObject Enemy;
  [Tooltip("The amount of tiles the player needs to move before this will spawn")]
  public int SpawnEnemyTurnInterval = 4;
  [Tooltip("Max amount of enemies that can be active at the same time")]
  public int MaxEnemiesAtOnce = 1;
  List<GameObject> enemies = new();
  EventManager eventManager;

  int turnNo = 0;

  private void Start()
  {
    eventManager = FindObjectOfType<EventManager>();
    eventManager.Register<PlayerMoveEvent>(OnPlayerMoved);

    // ensure we don't spawn anything until the player has at least once die
    var flagTracker = GetComponent<FlagTracker>();

    flagTracker.Count = 1;
    flagTracker.Flag = Flag.DICE_GAINED;
  }

  public void OnPlayerMoved(PlayerMoveEvent e)
  {
    turnNo++;
    if (turnNo % SpawnEnemyTurnInterval == 0)
    {
      if (FindFreeEnemySpawner(out Vector3 enemySpawnPos))
      {
        SpawnEnemyAt(enemySpawnPos);
      }
    }
  }

  public void SpawnEnemyAt(Vector3 coord)
  {
    //bail out if we have too many spawned
    if (transform.childCount >= MaxEnemiesAtOnce) return;

    var grid = GetGrid();
    var enemy = Instantiate(Enemy, transform);
    enemy.GetComponent<GridMovement>().Grid = grid;
    enemy.transform.position = coord;

  }

  Grid GetGrid()
  {
    var grids = FindObjectsOfType<Grid>();
    if (grids.Length > 1)
    {
      Debug.LogWarning("!!! More than one grid");
    }
    var grid = grids[0];
    return grid;
  }

  public bool FindFreeEnemySpawner(out Vector3 position)
  {
    var spawners = GameObject.FindGameObjectsWithTag("Spawner");
    position = Vector3.zero;

    foreach (var spawner in spawners)
    {
      var pos = spawner.transform.position;
      var spawnerTilePos = Utils.xy(GetGrid().WorldToCell(pos));
      bool spawnerEmpty = true;
      foreach (var enemy in enemies)
      {
        var tilePos = enemy.GetComponent<GridMovement>().CurrentTile;
        if (tilePos == spawnerTilePos)
        {
          spawnerEmpty = false;
          break;
        }
      }

      if (spawnerEmpty)
      {
        position = spawner.transform.position;
        return true;
      }
    }
    return false;
  }
}
