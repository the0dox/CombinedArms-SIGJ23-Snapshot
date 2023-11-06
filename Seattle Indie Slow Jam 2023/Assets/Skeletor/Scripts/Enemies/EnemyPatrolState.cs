using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by Skeletor
// default enemy behavior state, enemy will move from random position to random position until a player enters is radius of detection
public class EnemyPatrolState : State<EnemyBehavior>
{
    // radius of how far the enemy will patrol
    private const float WANDERRADIUS = 10;
    // generates a random position to patrol towards
    private Vector3 RandomWanderPosition => _myContext.transform.position + new Vector3(Random.Range(-WANDERRADIUS, WANDERRADIUS), 0, Random.Range(-WANDERRADIUS, WANDERRADIUS));


    // allows class that derive State to 
    protected override void OnStateEnter()
    {
        _myContext.MyAgent.enabled = true;
        _myContext.MyAgent.speed = 3.5f;
        _myContext.MyAgent.SetDestination(RandomWanderPosition);
    }

    // called every frame
    protected override void OnStateUpdate()
    {
        CheckDestination();
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

    void CheckDestination()
    {
        if(_myContext.MyAgent.remainingDistance < 0.02f)
        {
            _myContext.SetState(_myContext.Idle);
        }
    }
}
