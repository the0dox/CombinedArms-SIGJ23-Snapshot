using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by Skeletor
// enemy will approach the player and will get ready to make an attack against the player
public class EnemyRepositionState : State<EnemyBehavior>
{
    // the random time the enemy will wait to reposition
    public float NodeWaitTime => Random.Range(0.2f, 0.5f); 
    // the distance at which the enemy will attempt to stand away from the player 
    public float RepositionDistance => Random.Range(6, 14); 
    // the radius at which the enemy can reposition around the player 0 = no variation (enemy will only every walk directly towards player)
    private float _repositionRadius = 60;
    private float _repositionTimer;
    // allows class that derive State to 
    protected override void OnStateEnter()
    {
        if(_myContext.LookTarget == null)
        {
            _myContext.SetState(_myContext.Idle);
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
            _myContext.SetState(_myContext.Attack);
        }
    }

    void FindRepositionPoint()
    {
        // draws a vector from the target pointing to the enemy
        Vector3 targetDirection = (_myContext.transform.position - _myContext.LookTarget.position).normalized;

        targetDirection = Quaternion.Euler(0, Random.Range(-_repositionRadius, _repositionRadius), 0) * targetDirection;
        
        targetDirection *= RepositionDistance;

        Debug.DrawRay(_myContext.LookTarget.position, targetDirection, Color.blue);
        _repositionTimer = NodeWaitTime;
        _myContext.MyAgent.SetDestination(_myContext.LookTarget.position + targetDirection);
    }

    void CheckDestination()
    {
        if(_myContext.MyAgent.remainingDistance < 0.02f)
        {
            _repositionTimer -= Time.deltaTime;
            if(_repositionTimer < 0)
                FindRepositionPoint();
        }
    }

}
