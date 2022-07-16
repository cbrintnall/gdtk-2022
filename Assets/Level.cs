using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
[ExecuteInEditMode]
public class Level : MonoBehaviour
{
  public GameObject FloorTile;
  public Vector2Int Size;

  Grid grid;

  public void SetAtTile(int x, int y, Transform target) => SetAtTile(new Vector2Int(x, y), target);
  public void SetAtTile(Vector2Int tile, Transform target)
  {
    target.position = grid.CellToWorld(
      new Vector3Int(tile.x, 0, tile.y)
    );

    Utils.AlignToGrid(grid, target);
  }

  private void Awake()
  {
    grid = GetComponent<Grid>();
  }

  private void FixedUpdate()
  {
    if (Application.isEditor)
    {
      for(int i = 0; i < transform.childCount; i++)
      {
        Utils.AlignToGrid(grid, transform.GetChild(i));
      }
    }
  }
}
