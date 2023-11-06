using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by Skeletor
// a state in which the enemy will stand still for a period of time until deciding to patrol again
public class EnemyIdleState :  State<EnemyBehavior>
{
    // minimum time enemy can remain idel
    private const float MINIDLETIME = 2;
    // maximum time enemy can remain idle
    private const float MAXIDLETIME = 8;
    // duration in seconds the enemy has remaining idle
    private float idleTimer; 

    // called when state is first entered 
    protected override void OnStateEnter()
    {
        _myContext.MyAgent.enabled = false;
        idleTimer = Random.Range(MINIDLETIME, MAXIDLETIME);
    }

    // called every frame
    protected override void OnStateUpdate()
    {
        CheckTimer();
        _myContext.LookForPlayer();
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
        idleTimer -= Time.deltaTime;
        if(idleTimer < 0)
            _myContext.SetState(_myContext.Patrol);
    }
}
