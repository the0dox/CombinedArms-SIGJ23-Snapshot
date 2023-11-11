using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// created by skeletor
// state triggered when enemy is falling, tracks how long the enemy falls
public class EnemyFallingState : State<EnemyBehavior>
{
    private const float MINIMUMDAMAGINGVELOCITY = 3;
    // minimum time enemy can remain idel
    private const float FallingDamageMultipler = 1;

    // called when state is first entered 
    protected override void OnStateEnter()
    {
        _myContext.AnimationComponent.SetBool("Falling", true);
    }

    // called every frame
    protected override void OnStateUpdate()
    {
        if(_myContext.Grounded)
        {
            HitGround();
        }
    }

    // called every physics update frame
    protected override void OnStateFixedUpdate()
    {

    }

    // called when this state is ended
    protected override void OnStateExit()
    {
    }

    // called when the enemy hits the ground, deals damage if the enemy was moving fast enough, and exits state
    void HitGround()
    {
        _myContext.AnimationComponent.SetBool("Falling", false);
        float fallingVelocity = math.abs(_myContext.MyBody.velocity.y);
        if(fallingVelocity > MINIMUMDAMAGINGVELOCITY)
        {
            _myContext.TakeDamage(_myContext.transform.position + Vector3.up, fallingVelocity * FallingDamageMultipler);
        }
        else
        {
            _myContext.SetState(_myContext.Idle);
        }
    }
}
