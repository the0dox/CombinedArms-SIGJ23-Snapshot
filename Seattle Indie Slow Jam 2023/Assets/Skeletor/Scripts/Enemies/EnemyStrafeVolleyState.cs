using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// Fires off a volley of rounds while strafing to one side or another, maintaining speed from SEARCH. Engagement range medium.
public class EnemyStrafeVolleyState : State<EnemyBehavior>
{
    // the time (in seconds) between attacks
    private const float ATTACKWINDUP = 0.4f;
    // the number attacks total made in a volley
    private const float ATTACKCOUNT = 4f;
    // timer that tracks the time between attacks
    private float _attackTimer;
    // the number of attacks left in this current volley
    private float _remainingAttacks;
    // returns true if the enemy has faced the player for the first time
    private bool _targetAquired;
    // the traversal vector the enemy is moving across 
    private Vector3 strafeVelocity;

    // allows class that derive State to 
    protected override void OnStateEnter()
    {
        if(_myContext.LookTarget == null)
        {
            _myContext.SetState(_myContext.Idle);
            return;
        }
        _myContext.ToggleNavAgent(false);
        _myContext.AttackCoolDown = true;
        _targetAquired = false;
        //_myContext.transform.LookAt(new Vector3(_myContext.LookTarget.position.x ,_myContext.transform.position.y, _myContext.LookTarget.position.z));
        _myContext.AnimationComponent.SetTrigger("Attack");
        _attackTimer = ATTACKWINDUP;
        _remainingAttacks = ATTACKCOUNT;
    }

    // called every frame
    protected override void OnStateUpdate()
    {
        if(_targetAquired)
        {
            _attackTimer -= Time.deltaTime;
            if(_attackTimer < 0)
            {
                if(_remainingAttacks > 0 && _myContext.LockedOnTarget)
                {
                    Attack();
                    _attackTimer = ATTACKWINDUP;
                    _remainingAttacks--;
                }
                else
                {
                    _myContext.SetState(_myContext.Idle);
                }
            }
        }
    }

    // called every physics update frame
    protected override void OnStateFixedUpdate()
    {
        // first look at the player before strafing
        if(!_targetAquired)
        {
            bool lookingAtPlayer = _myContext.RotateTowardsTarget();
            if(lookingAtPlayer)
            {
                _targetAquired = true;
                SetStrafeVector();
            }
        }
        // stop moving if I hit a wall during my movement
        else if(Physics.Raycast(_myContext.transform.position, strafeVelocity.normalized, 1, LayerMask.GetMask("Terrain")))
        {
            _myContext.SetState(_myContext.Idle);
        }
        // else continue moving along my vector
        else
        {
            _myContext.transform.position += strafeVelocity * Time.deltaTime;
        }
    }

    // called when this state is ended
    protected override void OnStateExit()
    {
        _myContext.StartCoroutine(CoolDownDelay());
    }

    void SetStrafeVector()
    {
        Vector3 enemyToPlayer = (_myContext.LookTarget.position - _myContext.transform.position).normalized;
        float angle = Random.Range(60,300);
        strafeVelocity = Quaternion.Euler(0, angle, 0) * enemyToPlayer;
        strafeVelocity = new Vector3(strafeVelocity.x, 0, strafeVelocity.z);
        strafeVelocity *= _myContext.MyAgent.speed;
    }

    // tell the enemy that it cannot attack again for a brief period 
    IEnumerator CoolDownDelay()
    {
        yield return new WaitForSeconds(_myContext.AttackCoolDownDuration);
        _myContext.AttackCoolDown = false;
    }

    // create a single projectile traveling towards the player
    void Attack()
    {
        Vector3 attackVector = (_myContext.LookTarget.transform.position - _myContext.VisionTransform.position).normalized;
        GameObject projectile = ObjectLoader.LoadObject("Projectile");
        projectile.transform.position = _myContext.VisionTransform.position + (attackVector * 2);
        projectile.transform.rotation = Quaternion.LookRotation(attackVector);    
    }
}
