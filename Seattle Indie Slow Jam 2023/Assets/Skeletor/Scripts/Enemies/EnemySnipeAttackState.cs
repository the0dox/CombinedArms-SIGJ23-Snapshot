using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// state for an enemy attack where the enemy fires a very percise shot after training on the player for a long time
public class EnemySnipeAttackState : State<EnemyBehavior>
{
    // temporary variables
    private const float ATTACKWINDUP = 6f;
    private const float ATTACKDAMAGE = 40;

    private float _attackTimer;
    private LineRenderer sniperTrail;
    private bool _targetAquired;

    // allows class that derive State to 
    protected override void OnStateEnter()
    {
        if(_myContext.LookTarget == null)
        {
            _myContext.SetState(_myContext.Idle);
            return;
        }
        _myContext.ToggleNavAgent(false);
        
        _targetAquired = false;
    }

    // called every frame
    protected override void OnStateUpdate()
    {
        if(_targetAquired)
        {
            UpdateTrailRenderer();
            _attackTimer -= Time.deltaTime;
            if(_attackTimer < 0)
            {
                Attack();
            }
        }
    }

    // called every physics update frame
    protected override void OnStateFixedUpdate()
    {
        _myContext.RotateTowardsTarget();
        // first look at the player before strafing
        if(!_targetAquired)
        {
            if(_myContext.LockedOnTarget)
            {
                _targetAquired = true;
                sniperTrail = ObjectLoader.LoadObject("SniperTrailFX").GetComponent<LineRenderer>();
                _attackTimer = ATTACKWINDUP;
            }
        }
    }

    // called when this state is ended
    protected override void OnStateExit()
    {
        sniperTrail.gameObject.SetActive(false);
    }

    // makes a powerful hitscan attack against the player that can only stopped by terrain
    void Attack()
    {
        Vector3 attackVector = _myContext.LookTarget.transform.position - _myContext.VisionTransform.position;
        // if line of sight is not blocked by terrain
        if(!Physics.Raycast(_myContext.VisionTransform.position, attackVector, out RaycastHit hit, attackVector.magnitude, LayerMask.GetMask("Terrain")))
        {
            _myContext.LookTarget.TryGetComponent(out IAttackable target);
            target.TakeDamage(_myContext.VisionTransform.position, ATTACKDAMAGE);
        } 
        _myContext.AnimationComponent.SetTrigger("Attack");
        _myContext.TriggerAttackCoolDown();
        _myContext.SetState(_myContext.Approach);
        _myContext.PlaySound(_myContext.Loadout.ActiveWeapon.SoundFX);
    }

    // draws the trail renderer between the enemy and its target
    void UpdateTrailRenderer()
    {        
        Vector3 attackVector = _myContext.LookTarget.transform.position - _myContext.VisionTransform.position;
        Vector3 hitVector = _myContext.LookTarget.position;
        if(Physics.Raycast(_myContext.VisionTransform.position, attackVector, out RaycastHit hit, attackVector.magnitude, LayerMask.GetMask("Terrain")))
        {
            hitVector = hit.point;
        } 
        sniperTrail.SetPosition(0, _myContext.VisionTransform.position);
        sniperTrail.SetPosition(1, hitVector);
    }
}

