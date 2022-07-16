using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class DiceController : MonoBehaviour
{
    public string diceName;
    public string diceDesc;

    public List<BaseItem> items;

    public List<BaseItem> sortedItems;
    public Dictionary<string, BaseItem> itemsDic;
    public Dictionary<string, int> itemsCount;


    public void Awake()
    {
        sortedItems = new List<BaseItem>();
        itemsDic = new Dictionary<string, BaseItem>();
        itemsCount = new Dictionary<string, int>();

        foreach(BaseItem item in items)
        {
            addItem(item);
        }
    }

    public void addItem(BaseItem item)
    {
        string n = item.getName();
        if (itemsDic.ContainsKey(n))
        {
            itemsCount[n]++;
        } else
        {
            itemsDic[n] = item;
            itemsCount[n] = 1;

            sortedItems.Add(item);
            sortedItems = sortedItems.OrderBy(item => item.getPriority()).ToList();
        }
    }

    public TargetState getTargetState() {
        TargetState state = new TargetState();
        foreach(BaseItem item in sortedItems)
        {
            UnityEngine.Debug.Log(item.getPriority());
            item.updateTargetState(state);
        }
        return state;
    }

    public ProbState getProbState() {
        ProbState state = new ProbState();

        foreach(BaseItem item in sortedItems)
        {
            item.updateProbState(state);
        }
        return state;
    }

    public int weightedSample(List<double> probs)
    {
        List<double> cumSum = new List<double>();
        double x = 0;
        foreach (double p in probs)
        {
            x += p;
            cumSum.Add(x);
        }
        UnityEngine.Debug.Log(string.Join("; ", cumSum));

        int i;
        double r = Random.Range(0.0f, (float)x);
        for (i = 0; i < cumSum.Count; i++)
        {
            if (r <= cumSum[i])
            {
                break;
            }
        }

        return i;
    }

    public AttackState getAttackState() {
        ProbState probState = getProbState();
        UnityEngine.Debug.Log(string.Join(", ", probState.probs));

        int result = weightedSample(probState.probs) + 1;
        UnityEngine.Debug.Log($"Initial Result is {result}");

        AttackState attackState = new AttackState();
        attackState.rollResult = result;

        foreach (BaseItem item in sortedItems)
        {
            item.updateAttackState(attackState);
        }

        UnityEngine.Debug.Log($"Final Result is {attackState.rollResult}");

        return attackState;
    }

}
