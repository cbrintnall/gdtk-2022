using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dice Item", menuName = "Dice/Single Target Dice")]
public class SingleTargetDice : BaseItem
{
  public Vector2 Offset;

  public override string getName()
  {
    throw new NotImplementedException();
  }

  public override void onRoll(OnRollEvent e)
  {
    throw new NotImplementedException();
  }
}
