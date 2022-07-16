using System;
using UnityEngine;

public static class Utils 
{
  public static bool ToBool(this float fl) => Convert.ToBoolean(fl);
  public static void AlignToGrid(Grid grid, Transform transform)
  {
    var vec = GetTileForTransform(grid, transform);

    transform.position = new Vector3(vec.x, transform.position.y, vec.y);
  }

  public static Vector2Int GetTileForTransform(Grid grid, Transform transform)
  {
    Vector3 targetPosition = grid.CellToWorld(
      grid.WorldToCell(transform.position)
    ) + grid.cellSize / 2f;

    return new Vector2Int(Mathf.FloorToInt(targetPosition.x), Mathf.FloorToInt(targetPosition.z));
  }

  public static float Average(this Vector3 vec) => (vec.x + vec.y + vec.z) / 3f;
}