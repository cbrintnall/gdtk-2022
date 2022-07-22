using System;
using System.Collections.Generic;
using UnityEngine;
using Arc.Lib.Utils;
using UnityEngine.UI;
using System.Text;
using System.Linq;

namespace Arc.Lib.Debug 
{
  [AttributeUsage(AttributeTargets.Method)]
  class Command : Attribute { }

  class CommandHandler
  {
    string ValidHandlers => string.Join(",", handlers.Keys);
    Dictionary<string, Action<string[]>> handlers;

    public CommandHandler()
    {
      handlers = new Dictionary<string, Action<string[]>>()
      {
        ["cl"] = HandleCall,
        ["set"] = HandleSet,
      };
    }

    public void OnCommand(string command)
    {
      string[] split = command.Split(" ");

      if (split.Length <= 1)
      {
        UnityEngine.Debug.LogWarning($"Command {command} is invalid, separate by spaces and must begin with a recognized keyword; {ValidHandlers}");
        return;
      }else if (!handlers.Keys.Contains(split[0])){
        UnityEngine.Debug.LogWarning($"Handler {split[0]} is not a registered handler, try: {ValidHandlers}");
      }
    }

    private void HandleCall(string[] str)
    {

    }

    private void HandleSet(string[] str)
    {

    }
  }

  [SingletonLoader.Singleton]
  public class DebugManager : MonoBehaviour
  {
    [SerializeField] float _lineHeight = 20f;

    Dictionary<string, string> _data = new Dictionary<string, string>();

    StringBuilder command = new StringBuilder();
    bool active;

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Tilde))
      {
        active = !active;
      }
      else
      {
        foreach (char str in Input.inputString)
        {
          if (str == '\b')
          {
            if (command.Length > 0)
              command.Remove(command.Length - 1, 1);
          }
          else if ((str == '\n') || (str == '\r'))
          {
            SubmitCommand(command.ToString());
            command = new StringBuilder();
          }
          else
          {
            command.Append(str);
          }
        }
      }
    }

    public void SubmitCommand(string command)
    {
      print($"Submitting command: {command}");
    }

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
      if(!active) return;

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

      GUI.Label(
        new Rect(
          0f,
          Screen.height - _lineHeight,
          Screen.width,
          _lineHeight*3f
        ),
        command.ToString()
      );
    }
  }
}