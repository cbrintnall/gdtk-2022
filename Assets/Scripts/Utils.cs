using System;
using UnityEngine;

public static class Utils 
{
  public static bool ToBool(this float fl) => Convert.ToBoolean(fl);
  public static void AlignToGrid(Grid grid, Transform transform)
  {
    Vector3 targetPosition = grid.CellToWorld(
      grid.WorldToCell(transform.position)
    ) + grid.cellSize/2f;

    transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
  }
}