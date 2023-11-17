using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by Skeletor
// the state in which the enemy make an a attack and a target
public class EnemyAttackState : State<EnemyBehavior>
{
    // temporary variables
    private const float ATTACKWINDUP = 1f;

    private float _attackTimer;

    // allows class that derive State to 
    protected override void OnStateEnter()
    {
        if(_myContext.LookTarget == null)
        {
            _myContext.SetState(_myContext.Idle);
            return;
        }
        _myContext.ToggleNavAgent(false);
        
        //_myContext.transform.LookAt(new Vector3(_myContext.LookTarget.position.x ,_myContext.transform.position.y, _myContext.LookTarget.position.z));
        _myContext.AnimationComponent.SetTrigger("Attack");
        _attackTimer = ATTACKWINDUP;
    }

    // called every frame
    protected override void OnStateUpdate()
    {
        _attackTimer -= Time.deltaTime;
        if(_attackTimer < 0)
        {
            Attack();
        }
    }

    // called every physics update frame
    protected override void OnStateFixedUpdate()
    {
        _myContext.RotateTowardsTarget();
    }

    // called when this state is ended
    protected override void OnStateExit(){}

    void Attack()
    {
        Vector3 attackVector = (_myContext.LookTarget.transform.position - _myContext.VisionTransform.position).normalized;
        GameObject projectile = ObjectLoader.LoadObject("Projectile");
        projectile.transform.position = _myContext.VisionTransform.position + (attackVector * 2);
        projectile.transform.rotation = Quaternion.LookRotation(attackVector);   
        _myContext.TriggerAttackCoolDown(); 
        _myContext.SetState(_myContext.Approach);
        _myContext.PlaySound(_myContext.Loadout.ActiveWeapon.SoundFX);
    }
}

