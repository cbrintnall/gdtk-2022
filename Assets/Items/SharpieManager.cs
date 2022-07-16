using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpieManager : BaseItem
{
    public int fromNumber;
    public int toNumber;

    public override string getName()
    {
        return "Sharpie";
    }

    public override void onRoll(OnRollEvent e)
    {
        if (e.dice == dice && e.result == fromNumber)
        {
            UnityEngine.Debug.Log($"Replacing the roll {fromNumber}-->{toNumber}");
            e.result = toNumber;
        }
    }
}
