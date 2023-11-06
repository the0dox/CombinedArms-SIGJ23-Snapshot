using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by Skeletor
// enemy will approach the player and will get ready to make an attack against the player
public class EnemyRepositionState : State<EnemyBehavior>
{
    // the distance at which the enemy will attempt to stand away from the player 
    private float _repositionDistance = 9;
    // the radius at which the enemy can reposition around the player 0 = no variation (enemy will only every walk directly towards player)
    private float _repositionRadius = 90;
    // allows class that derive State to 
    protected override void OnStateEnter()
    {
        if(_myContext.LookTarget == null)
        {
            _myContext.SetState(_myContext.idle);
            return;
        }
        _myContext.MyAgent.enabled = true;
        FindRepositionPoint();
    }

    // called every frame
    protected override void OnStateUpdate()
    {
        CheckDestination();
        CheckAttackDistance();
    }

    // called every physics update frame
    protected override void OnStateFixedUpdate()
    {

    }

    // called when this state is ended
    protected override void OnStateExit()
    {

    }

    // checks to see if a look target is within attack range
    void CheckAttackDistance()
    {
        if(_myContext.LookTarget == null)
            return;
        if(Vector3.Distance(_myContext.transform.position, _myContext.LookTarget.position) < _myContext.AttackRadius && !_myContext.AttackCoolDown)
        {
            _myContext.SetState(_myContext.attack);
        }
    }

    void FindRepositionPoint()
    {
        // draws a vector from the target pointing to the enemy
        Vector3 targetDirection = (_myContext.transform.position - _myContext.LookTarget.position).normalized;

        targetDirection = Quaternion.Euler(0, Random.Range(-_repositionRadius, _repositionRadius), 0) * targetDirection;
        
        targetDirection *= _repositionDistance;

        Debug.DrawRay(_myContext.LookTarget.position, targetDirection, Color.blue);

        _myContext.MyAgent.SetDestination(_myContext.LookTarget.position + targetDirection);
    }

    void CheckDestination()
    {
        if(_myContext.MyAgent.remainingDistance < 0.02f)
        {
            FindRepositionPoint();
        }
    }
}
