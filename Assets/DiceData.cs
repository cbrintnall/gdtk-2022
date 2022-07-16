using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OnRollEvent : BaseEvent
{
    public DiceData dice;
    public int result;
}

public class DiceData : MonoBehaviour
{
    public string id;
    public string description;
    public int numSides;
    public List<BaseItem> items;
    public Dictionary<GameObject, BaseItem> itemDic;

    private EventManager eventManager;


    public void Awake()
    {
        items = new List<BaseItem>();
        itemDic = new Dictionary<GameObject, BaseItem>();
        eventManager = FindObjectOfType<EventManager>();
    }

    public void roll()
    {
        // TODO: actually getting the initial result will be different now
        int result = Random.Range(1, numSides + 1);
        UnityEngine.Debug.Log($"Initial roll is {result}");

        OnRollEvent e = new OnRollEvent { dice = this, result = result };
        eventManager.Publish(e);

        UnityEngine.Debug.Log($"Resulting result is {e.result}");

    foreach(var item in this.items)
    {
      item.onRoll(e);
    }
    }



}
