using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    private protected int count;
    private protected DiceData dice;
    EventManager eventManager;

    public void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        count = 1;
    }

    public static void assignToDice(GameObject itemPrefab, DiceData dice)
    {
        BaseItem item;
        if (dice.itemDic.TryGetValue(itemPrefab, out item))
        {
            item.count++;
            UnityEngine.Debug.Log($"{item.getName()} already was assigned to {dice.id}, increasing its count to {item.count}");
        } else
        {
            GameObject o = Instantiate(itemPrefab);
            item = o.GetComponent<BaseItem>();
            UnityEngine.Debug.Log($"Assigning {item.getName()} to {dice.id}");

            dice.itemDic[itemPrefab] = item;
            dice.items.Add(item);
            item.dice = dice;

            item.eventManager.Register<OnRollEvent>(item.onRoll);
        }
    }

    public int getCount()
    {
        return count;
    }

    public abstract string getName();

    public abstract void onRoll(OnRollEvent e);
}
