using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Arc.Lib.Utils
{
  public class SingletonLoader
  {
    [AttributeUsage(AttributeTargets.Class)]
    public class Singleton : Attribute 
    { }

    private static Dictionary<Type, object> Singletons = new();

    public static T Get<T>() where T : class
    {
      var type = typeof(T);

      if (Singletons.ContainsKey(type))
      {
        var singleton = Singletons[type];

        if (singleton is T castedSingleton)
        {
          return castedSingleton;
        }
      }

      return null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void LoadSingletons()
    {
      foreach(Type singleton in GetTypesInAssemblyWithAttribute(typeof(Singleton)))
      {
        if (singleton.IsSubclassOf(typeof(MonoBehaviour)))
        {
          string name = singleton.ToString();

          if (GameObject.Find(name) == null)
          {
            GameObject newSingleton = new GameObject(name);

            object component = newSingleton.AddComponent(singleton);

            GameObject.DontDestroyOnLoad(newSingleton);

            UnityEngine.Debug.Log($"Created behaviour singleton {name}");

            Singletons[singleton] = component;
          }
        }
        else
        {
          UnityEngine.Debug.LogWarning($"Tried to create singleton of type {singleton}, but does not derive from MonoBehaviour");
        }
      }
    }

    public static List<Type> GetTypesInAssemblyWithAttribute(Type attribute)
    {
      return AppDomain.CurrentDomain.GetAssemblies()
        .Select(assembly => assembly.GetTypes())
        .SelectMany(types => types.Where(type => type.GetCustomAttributes(attribute, true).Count() > 0))
        .ToList();
    }
  }
}