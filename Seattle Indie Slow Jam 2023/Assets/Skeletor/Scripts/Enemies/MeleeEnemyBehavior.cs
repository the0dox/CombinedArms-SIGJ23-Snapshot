using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeEnemyBehavior : EnemyBehavior
{
    // public accessor
    public GameObject MeleeHitBox => _meleeHitBox;
    // reference to the attack hit box of melee strike
    [SerializeField] private GameObject _meleeHitBox;

    protected override void OnSpawn()
    {
        _patrol = new EnemyPatrolState();
        _idle = new EnemyIdleState();
        _approach = new EnemyRepositionState();
        _attack = new EnemyMeleeAttackState();
        _hurt = new EnemyInjuredState();
        _meleeHitBox.SetActive(false);
    }

    // called by the animation controller when the attack animation is complete 
    public void AttackEnded()
    {
        SetState(Idle);
    }

    // called by the animation controller when the attack animation has reached a point where the player can be damaged
    public void AttackStarted()
    {
        _meleeHitBox.SetActive(true);
        // there is a possibility this enemy has fallen of a ledge trying to reach the player. If that is the case they should start playing the fall animation
        if(!Grounded)
            AnimationComponent.SetBool("Falling", true);
    }

    // melee enemies do not drop anything on death at the moment
    protected override void OnDeath(Vector3 source){}
}
