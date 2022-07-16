using System;
using System.Collections;
using UnityEngine;

public static class Utils 
{
  public static Vector2Int xy(Vector3Int xyz)
  {
    return new Vector2Int(xyz.x, xyz.z);
  }
  public static IEnumerator Defer(Action cb)
  {
    yield return null;
    cb.Invoke();
  }

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

  public static T Random<T>(this T[] arr) => arr[UnityEngine.Random.Range(0, arr.Length-1)];
  public static float Average(this Vector3 vec) => (vec.x + vec.y + vec.z) / 3f;
}