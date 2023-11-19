using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

// created by skeletor
// state for an enemy attack where the enemy fires a very percise shot after training on the player for a long time
public class EnemySnipeAttackState : State<EnemyBehavior>
{
    // temporary variables
    private const float ATTACKWINDUP = 5f;
    private const float WARNINGPERIOD = 0.4f;
    private const float ATTACKDAMAGE = 40;

    private float _attackTimer;
    private LineRenderer sniperTrail;
    private bool _targetAquired;
    private bool _warningTriggered;

    // allows class that derive State to 
    protected override void OnStateEnter()
    {
        if(_myContext.LookTarget == null)
        {
            _myContext.SetState(_myContext.Idle);
            return;
        }
        _myContext.ToggleNavAgent(false);
        _warningTriggered = false;
        _targetAquired = false;

    }

    // called every frame
    protected override void OnStateUpdate()
    {
        if(_targetAquired)
        {
            UpdateTrailRenderer();
            _attackTimer -= Time.deltaTime;
            if(_attackTimer < WARNINGPERIOD && !_warningTriggered)
            {
                Warning();
            }
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
                sniperTrail.widthMultiplier = 1;
                sniperTrail.startColor = Color.red;
                _attackTimer = ATTACKWINDUP;
            }
        }
    }

    // called when this state is ended
    protected override void OnStateExit()
    {
        if(sniperTrail != null)
        {
            sniperTrail.gameObject.SetActive(false);
        }
        sniperTrail = null;
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
        GameObject smokeParticles = ObjectLoader.LoadObject("SmokeFX", true);
        smokeParticles.transform.SetParent(_myContext.Loadout.ActiveWeapon.transform);
        smokeParticles.transform.localPosition = Vector3.zero;
        smokeParticles.transform.rotation = Quaternion.identity;
        _myContext.TriggerAttackCoolDown();
        _myContext.SetState(_myContext.Approach);
        _myContext.PlaySound(_myContext.Loadout.ActiveWeapon.SoundFX);
    }

    void Warning()
    {
        sniperTrail.startColor = Color.yellow;
        _warningTriggered = true;
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
        sniperTrail.widthMultiplier = Mathf.Lerp(sniperTrail.widthMultiplier, 0, Time.deltaTime * (1/ATTACKWINDUP));
    }
}

