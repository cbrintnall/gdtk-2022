using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utils 
{
  public static void ListenForEvent<T>(this EventTrigger trigger, EventTriggerType type, Action<T> cb) where T : BaseEventData
  {
    EventTrigger.Entry entry = new EventTrigger.Entry();
    entry.eventID = type;
    entry.callback.AddListener((data) => { cb(data as T); });
    trigger.triggers.Add(entry);
  }

  public static void Shuffle<T>(this List<T> list)
  {
    int n = list.Count;
    while (n > 1)
    {
      n--;
      int k = UnityEngine.Random.Range(0, n + 1);
      T value = list[k];
      list[k] = list[n];
      list[n] = value;
    }
  }

  public static Vector3Int xyz(Vector2Int xz)
  {
    return new Vector3Int(xz.x, 0, xz.y);
  }

  public static Vector2Int xy(Vector3Int xyz)
  {
    return new Vector2Int(xyz.x, xyz.z);
  }

  public static IEnumerator Defer(Action cb)
  {
    yield return null;
    cb.Invoke();
  }

  public static IEnumerator DelaySecondsThen(float seconds, Action cb)
  {
    yield return new WaitForSeconds(seconds);
    cb();
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