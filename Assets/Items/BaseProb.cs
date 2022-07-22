using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new BaseProb", menuName = "Items/BaseProb")]
public class BaseProb : BaseItem
{
  public int numSides;

  public override int getPriority() { return 2000; }

  public override bool HandleAssignToDice(DiceController dc)
  {
    return true;
  }

  public override void updateProbState(ProbState state)
  {
    double p = 1.0 / numSides;
    List<double> probs = new List<double>();

    for (int i=0; i < numSides; i++)
    {
        probs.Add(p);
    }

    state.probs = probs;
  }
}
