using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// a state that the enemy enters when recieving damage,
// the enemy will be stunned for a short period of time before resuming regular behavior
public class EnemyInjuredState : State<EnemyBehavior>
{
    // radius of how far the enemy will patrol
    private const float STUNDURATION = 0.5f;
    private float _stunTimer;
    // allows class that derive State to 
    protected override void OnStateEnter()
    {
        _myContext.MyAgent.enabled = false;
        _myContext.AnimationComponent.SetTrigger("Hurt");
        _stunTimer = STUNDURATION;
    }

    // called every frame
    protected override void OnStateUpdate()
    {
        CheckStun();
    }

    // called every physics update frame
    protected override void OnStateFixedUpdate()
    {

    }

    // called when this state is ended
    protected override void OnStateExit()
    {

    }

    // checks the stun timer to see if the enemy can reactivate, note that an enemy can only exit if they are grounded
    void CheckStun()
    {
        _stunTimer -= Time.deltaTime;
        if(_stunTimer < 0)
        {
            if(Physics.Raycast(_myContext.transform.position, Vector3.down, 0.55f, LayerMask.GetMask("Terrain")))
            {
                _myContext.SetState(_myContext.Idle);
            }
            else
            {
                Debug.Log("not grounded, checking again");
                _stunTimer = STUNDURATION;
            }
        }
    }
}
