using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

// created by skeletor
// state a melee enemy uses to lunge and attack the player in close range
public class EnemyMeleeAttackState : State<EnemyBehavior>
{
    // the maximum angle the enemy can rotate to try and catch the player
    private const float MAXANGLE = 45;
    // determines how fast (in seconds) the enemy will attempt to reach the player from their position when entering this stat
    private const float ATTACKDURATION = 0.5f;
    // determines how much faster the enemy will move if the player is moving away from them
    private const float OVERSHOTFACTOR = 2f;
    // my context needs to be casted once to get additional information specific to the melee enemy
    private MeleeEnemyBehavior _myConvertedContext;
    // the distance vector the enemy moves along for the duration of the attack
    private Vector3 AttackVelocity;
    
    // called when this state is started
    protected override void OnStateEnter()
    {
        if(_myConvertedContext == null)
        {
            _myConvertedContext = _myContext as MeleeEnemyBehavior;
        }
        _myContext.ToggleNavAgent(false);
        _myContext.AnimationComponent.SetTrigger("Attack");
        SetInterceptionPoint();
    }

    // called every frame whilist this is the active state
    protected override void OnStateUpdate()
    {
        _myContext.ApplyRootMotion();
    }

    // called every physics update frame
    protected override void OnStateFixedUpdate()
    {
        _myContext.transform.position += AttackVelocity * Time.deltaTime;
    }


    // called when this state is ended
    protected override void OnStateExit()
    {
        _myConvertedContext.MeleeHitBox.SetActive(false);       
        _myContext.AnimationComponent.SetBool("Falling", false);
    }

    // determines where the enemy should move for the duration of the attack animation
    // draws a triangle based of off current location, current player location, and anticipated player location and moves towards anticipation point
    void SetInterceptionPoint()
    {
        Vector3 playerPosition = _myContext.LookTarget.transform.position;
        Vector3 playerToEnemy = _myContext.transform.position - playerPosition;
        Vector3 playerToDestination =  PlayerManager.CurrentVelocity * ATTACKDURATION;
        float playerAngle = Vector3.SignedAngle(playerToEnemy, playerToDestination, Vector3.up); 
        Vector3 playerDestinationToEnemy = _myContext.transform.position - (playerPosition + playerToDestination);
        float anticipationAngle = Vector3.SignedAngle(playerDestinationToEnemy, -playerToDestination, Vector3.up);
        float enemyAngle = playerAngle - anticipationAngle + ( playerAngle > 0 ? -180 : 180 );
        AttackVelocity = -playerToEnemy;
        // if enemy is going to miss because the angle is too great, just attack directly
        if(Math.Abs(enemyAngle) <= MAXANGLE)
        {
            AttackVelocity = Quaternion.Euler(0, enemyAngle, 0) * AttackVelocity;
        }
        // if player is moving away from the player, increase velocity to catch up!
        if(Math.Abs(playerAngle) > MAXANGLE * 2)
        {
            Debug.Log("player backpedaling increase speed");
            AttackVelocity *= OVERSHOTFACTOR;
        }
        AttackVelocity = new Vector3(AttackVelocity.x, 0, AttackVelocity.z) * ATTACKDURATION;
        _myContext.transform.rotation = Quaternion.LookRotation(AttackVelocity, Vector3.up);
        /*
        Debug.DrawRay(playerPosition, playerToEnemy, Color.red);
        Debug.DrawRay(playerPosition, playerHeading, Color.green);
        Debug.DrawRay(playerPosition + playerHeading, playerDestinationToEnemy, Color.blue);
        Debug.LogFormat($"playertToEnemy angle {playerAngle} playerDestinationToEnemy angle {anticipationAngle} enemy to destination angle {enemyAngle}");

        Debug.DrawRay(_myContext.VisionTransform.position, AttackVelocity, Color.magenta);

        //Debug.Break();
        */
    }

}
