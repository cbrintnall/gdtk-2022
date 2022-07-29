using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new AreaTarget", menuName = "Items/AreaTarget")]
public class AreaTarget : BaseItem
{
  public int radius;

  public override int getPriority() { return 0; }

  public override bool HandleAssignToDice(DiceController dc) => true;

  public override void updateTargetState(TargetState state)
  {
    List<Vector2Int> t = new List<Vector2Int>();
    int r = radius / 2;
    for (int i = -r; i <= r; i++)
    {
        for (int j = -r; j <= r; j++)
        {
            t.Add(new Vector2Int(i, j));
        }
    }

    state.relativeTargets = t;
  }   
}
