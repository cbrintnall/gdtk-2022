using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Sharpie", menuName = "Items/Sharpie")]
public class SharpieManager : BaseItem
{
  public int fromNum;
  public int toNum;

  public override int getPriority() { return 2001; }
  public override bool HandleAssignToDice(DiceController dc) => true;
  public override void updateAttackState(AttackState state)
    {
        if (state.rollResult == fromNum)
        {
            UnityEngine.Debug.Log($"Replacing result from {fromNum}-->{toNum}");
            state.rollResult = toNum;
        }
    }
}
