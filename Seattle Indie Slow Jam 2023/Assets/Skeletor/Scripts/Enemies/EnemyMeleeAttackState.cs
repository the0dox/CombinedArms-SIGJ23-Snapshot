using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// state a melee enemy uses to lunge and attack the player in close range
public class EnemyMeleeAttackState : State<EnemyBehavior>
{
    // the amount of time between the enemy starting the attack and ending their swing
    private const float ATTACKDURATION = 1.5f;
    // the total duration of the attack state
    private const float STATEDURATION = 2f;
    // number of frames this state lasts
    private float currentAttackDuration;
    // the fastest the enemy can move while trying to collide with the player
    private const float ATTACKVELOCITYBASE = 6f;
    // a build in inaccuracy of where the the enemy will predict the player will end up
    private const float ANTICIPATIONERRORBASE = 4;

    private Vector3 AttackVelocity;
    
    // called when this state is started
    protected override void OnStateEnter()
    {
        _myContext.MyAgent.enabled = false;
        currentAttackDuration = STATEDURATION;
        SetInterceptionPoint();
    }

    // called every frame whilist this is the active state
    protected override void OnStateUpdate()
    {
        currentAttackDuration -= Time.deltaTime;
        if(currentAttackDuration < 0)
        {
            _myContext.SetState(_myContext.Idle);
        }
    }

    // called every physics update frame
    protected override void OnStateFixedUpdate()
    {
         _myContext.MyBody.velocity = AttackVelocity; 
    }


    // called when this state is ended
    protected override void OnStateExit()
    {

    }

    // anticipates where the player will be at the end of its attack, and homes in on that positition 
    void SetInterceptionPoint()
    {
        Vector3 playerPosition = _myContext.LookTarget.transform.position;
        Vector3 playerVelocity = _myContext.LookTarget.forward;
        float anticipationError = ANTICIPATIONERRORBASE;
        float MaxAttackVelocity = ATTACKVELOCITYBASE; 
        Vector3 anticipatedPosition = playerPosition + (playerVelocity * ATTACKDURATION) + new Vector3(Random.Range(-anticipationError,anticipationError), 0, Random.Range(-anticipationError,anticipationError));
        Vector3 targetVelocity = (anticipatedPosition - _myContext.transform.position)/ATTACKDURATION;
        if(targetVelocity.magnitude > MaxAttackVelocity)
        {
            targetVelocity = AttackVelocity.normalized * MaxAttackVelocity;
        }
        AttackVelocity = new Vector3(targetVelocity.x, 0, targetVelocity.z);
        _myContext.transform.LookAt(new Vector3(anticipatedPosition.x, _myContext.transform.position.y, anticipatedPosition.z));

    }
}
