using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Grid))]
[ExecuteInEditMode]
public class Level : MonoBehaviour
{
  Grid grid;

  public Grid GetGrid()
  { return grid; }

  private void Awake()
  {
    grid = GetComponent<Grid>();
  }

  public void SetAtTile(int x, int y, Transform target) => SetAtTile(new Vector2Int(x, y), target);

  public void SetAtTile(Vector2Int tile, Transform target)
  {
    target.position = grid.CellToWorld(
      new Vector3Int(tile.x, 0, tile.y)
    );

    Utils.AlignToGrid(grid, target);
  }

  public T GetObjectAtTile<T>(Vector2Int tile) where T : Component
  {
    return GetObjectsAtTile(tile)
      .Where(go => go.GetComponent<T>() != null)
      .Select(go => go.GetComponent<T>())
      .FirstOrDefault();
  }

  public List<GameObject> GetObjectsAtTile(Vector2Int tile) => TestForTile(tile, transform);

  private List<GameObject> TestForTile(Vector2Int tile, Transform transform)
  {
    List<GameObject> atTiles = new();

    for (int i = 0; i < transform.childCount; i++)
    {
      Transform childTransform = transform.GetChild(i);

      atTiles.AddRange(TestForTile(tile, childTransform));

      Vector2Int childTile = Utils.GetTileForTransform(grid, childTransform);

      if (tile == childTile)
      {
        atTiles.Add(childTransform.gameObject);
      }
    }

    return atTiles;
  }
}
