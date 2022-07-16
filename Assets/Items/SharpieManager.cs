using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Sharpie", menuName = "Items/Sharpie")]
public class SharpieManager : BaseItem
{
    public int fromNum;
    public int toNum;

    override public string getName() { return "Sharpie"; }
    override public string getDesc() { return "Replaces a value"; }

    public override int getPriority() { return 2001; }

    override public bool handleAssignToDice(DiceController dice)
    {
        return !dice.itemsDic.ContainsKey(getName());
    }

    public override void updateAttackState(AttackState state)
    {
        if (state.rollResult == fromNum)
        {
            UnityEngine.Debug.Log($"Replacing result from {fromNum}-->{toNum}");
            state.rollResult = toNum;
        }
    }
}
