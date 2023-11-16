using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeEnemyBehavior : EnemyBehavior
{
    // public accessor
    public GameObject MeleeHitBox => _meleeHitBox;
    // sound played during a melee Attack
    [SerializeField] private AudioClip _meleeSFX;
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
        PlaySound(_meleeSFX);
        _meleeHitBox.SetActive(true); 
        // there is a possibility this enemy has fallen of a ledge trying to reach the player. If that is the case they should start playing the fall animation
        if(!Grounded)
            AnimationComponent.SetBool("Falling", true);
    }

    // melee enemies do not drop anything on death at the moment
    protected override void OnDeath()
    {
         AudioSource.PlayClipAtPoint(DeathSound, _visionTransform.position, 3);
    }
}
