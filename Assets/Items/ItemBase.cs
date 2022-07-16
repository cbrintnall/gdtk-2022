using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class BaseItem : ScriptableObject
{

    public string assignToDice(DiceController dice)
    {
        UnityEngine.Debug.Log($"attempting to assign {getName()} to {dice.diceName}");

        BaseItem item;
        if (!dice.itemsDic.TryGetValue(getName(), out item)) { item = this; }

        if (item.handleAssignToDice(dice))
        {
            dice.addItem(item);
            UnityEngine.Debug.Log($"Added {getName()} to {dice.diceName}");
            return "added";
        }
        UnityEngine.Debug.Log($"Rejected {getName()} to {dice.diceName}");
        return "rejected";
    }

    public abstract string getName();
    public abstract int getPriority();
    public virtual string getDesc() { return ""; }

    public virtual bool handleAssignToDice(DiceController dice) { return true; }
    public virtual void updateTargetState(TargetState state) { }
    public virtual void updateProbState(ProbState state) { }
    public virtual void updateAttackState(AttackState state) { }

}
