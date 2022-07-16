using System;
using System.Collections.Generic;
using UnityEngine;
using Arc.Lib.Utils;

namespace Arc.Lib.Debug 
{
  [SingletonLoader.Singleton]
  public class DebugManager : MonoBehaviour
  {
    [SerializeField] float _lineHeight = 20f;

    Dictionary<string, string> _data = new Dictionary<string, string>();

    public void Track(string name, object data)
    {
      _data[name] = data.ToString();
    }

    /// <summary>
    /// A wrapper that allows placing Debug calls anywhere, but hiding them behind the
    /// debug toggle menu. Use the Debug namespace draw calls within your callbacks
    /// </summary>
    /// <param name="cb"></param>
    public void DoDebug(Action cb)
    {
      if (enabled)
      {
        cb();
      }
    }

    public void DebugRaycast(Vector3 start, Vector3 dir, bool hit = false, float time = 1f)
    {
      DoDebug(() =>
      {
        UnityEngine.Debug.DrawRay(start, dir, hit ? Color.green : Color.red, time);
      });
    }

    private void OnGUI()
    {
      int y = 0;

      foreach(KeyValuePair<string, string> data in _data)
      {
        GUI.Label(
          new Rect(
            new Vector2(0, y * _lineHeight),
            new Vector2(Screen.width, _lineHeight)
          ),
          $"{data.Key}: {data.Value}"
        );

        y++;
      }
    }
  }
}