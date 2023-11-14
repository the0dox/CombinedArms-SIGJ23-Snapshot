using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by Skeletor
// a state in which the enemy will stand still for a period of time until deciding to patrol again
public class EnemyIdleState :  State<EnemyBehavior>
{
    // time in seconds the enemy will check line of sight
    private const float VISIONINTERVAL = 0.2f;
    //time enemy stays in idle
    private FloatRange IdleTime = new FloatRange(2,3);
    // duration in seconds the enemy has remaining idle
    private float idleTimer;
    // duration in seconds the enemy has to wait before checking line of sight again 
    private float visionTimer;

    // called when state is first entered 
    protected override void OnStateEnter()
    {
        _myContext.ToggleNavAgent(false);
        idleTimer = IdleTime.RandomValue;
        visionTimer = 0;
    }

    // called every frame
    protected override void OnStateUpdate()
    {
        if(_myContext.Grounded)
        {
            CheckTimer();
        }
        else
        {
            _myContext.SetState(_myContext.Falling);
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

    // tick down idle timer and swtich to patrol if timer is complete
    void CheckTimer()
    {
        visionTimer -= Time.deltaTime;
        if(visionTimer < 0)
        {
            visionTimer = VISIONINTERVAL;
            _myContext.LookForPlayer();
        }
        else
        {
            idleTimer -= Time.deltaTime;
            if(idleTimer < 0)
                _myContext.SetState(_myContext.Patrol);
        }
    }
}
