using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyBehavior : EnemyBehavior
{
    protected override void OnSpawn()
    {
        _patrol = new EnemyPatrolState();
        _idle = new EnemyIdleState();
        _approach = new EnemyRepositionState();
        _attack = new EnemyMeleeAttackState();
        _hurt = new EnemyInjuredState();
    }
}
