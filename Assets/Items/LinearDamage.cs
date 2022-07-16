using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new LinearDamage", menuName = "Items/LinearDamage")]
public class LinearDamage : BaseItem
{
    public override string getName() { return "_linear_damage"; }

    public override int getPriority() { return 3000; }

    public override void updateAttackState(AttackState state)
    {
        state.damage = state.rollResult;
    }
}

