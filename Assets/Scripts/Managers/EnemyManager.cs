using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

[Singleton]
public class EnemyManager : MonoBehaviour
{
  public int SpawnEnemyTurnInterval = 4;

  List<GameObject> enemies = new();

  EventManager eventManager;
  LevelManager levelManager;

  GameObject enemy0;

  int turnNo = 0;

  private void Start()
  {
    enemy0 = Resources.Load<GameObject>("Enemy");
    levelManager = FindObjectOfType<LevelManager>();
    eventManager = FindObjectOfType<EventManager>();
    eventManager.Register<PlayerMoveEvent>(OnPlayerMoved);
  }

  public void OnPlayerMoved(PlayerMoveEvent e)
  {
    turnNo++;
    if (turnNo % 4 == 0)
    {
      if (FindFreeEnemySpawner(out Vector3 enemySpawnPos))
      {
        SpawnEnemyAt(enemySpawnPos);
      }
    }
  }

  public void SpawnEnemyAt(Vector3 coord)
  {
    var grid = GetGrid();
    var enemy = Instantiate(enemy0, grid.transform);
    enemy.GetComponent<GridMovement>().Grid = grid;
    enemy.transform.position = coord;

  }

  Grid GetGrid()
  {
    var grids = FindObjectsOfType<Grid>();
    if (grids.Length > 1)
    {
      Debug.Log("!!! More than one grid");
    }
    var grid = grids[0];
    return grid;
  }

  public bool FindFreeEnemySpawner(out Vector3 position)
  {
    levelManager = FindObjectOfType<LevelManager>();
    var spawners = GameObject.FindGameObjectsWithTag("Spawner");
    position = Vector3.zero;

    Debug.Log(spawners.Length);
    Debug.Log(spawners[0]);

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
