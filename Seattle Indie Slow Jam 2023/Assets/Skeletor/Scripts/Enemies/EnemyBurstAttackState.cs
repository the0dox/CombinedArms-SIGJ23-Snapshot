using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// enemy will look at player and shoot a burst of small projectiles in a cone
public class EnemyBurstAttackState: State<EnemyBehavior>
{
    // time before the enemy fires after entering the state
    private const float ATTACKWINDUP = 2f;
    // the time before the enemy resumes regular behavior after attacking
    private const float ATTACKWINDDOWN = 0.25f;
    // the width of the weapon spread in radians
    private const float WEAPONSPREAD = 30;
    // the number of projectiles fired in a single burst
    private const float NUMBEROFPELLETS = 10;

    private float _attackTimer;

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
        //_myContext.transform.LookAt(new Vector3(_myContext.LookTarget.position.x ,_myContext.transform.position.y, _myContext.LookTarget.position.z));
        _myContext.AnimationComponent.SetTrigger("Attack");
        _attackTimer = ATTACKWINDUP;
    }

    // called every frame
    protected override void OnStateUpdate()
    {
        _attackTimer -= Time.deltaTime;
        if(_attackTimer < 0)
        {
            FireSpread();
        }
    }

    // called every physics update frame
    protected override void OnStateFixedUpdate()
    {
        _myContext.RotateTowardsTarget();
    }

    // called when this state is ended
    protected override void OnStateExit()
    {
        _myContext.StartCoroutine(CoolDownDelay());
    }

    IEnumerator CoolDownDelay()
    {
        yield return new WaitForSeconds(_myContext.AttackCoolDownDuration);
        _myContext.AttackCoolDown = false;
    }

    void FireSpread()
    {
        Vector3 attackVector = (_myContext.LookTarget.transform.position - _myContext.VisionTransform.position).normalized;
        for(int i = 0; i < NUMBEROFPELLETS; i++)
        {
            float ang = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float yaw = Random.Range(-WEAPONSPREAD, WEAPONSPREAD) * Mathf.Deg2Rad;
            Vector3 spread = new Vector3(Mathf.Sin(yaw)* Mathf.Cos(ang)
            , Mathf.Sin(yaw)* Mathf.Sin(ang), Mathf.Sin(yaw)* Mathf.Cos(yaw));
            Attack(attackVector + spread);
        }
        _myContext.SetState(_myContext.Approach);    
        _myContext.PlaySound(_myContext.Loadout.ActiveWeapon.SoundFX);
    }

    void Attack(Vector3 attackVector)
    {
        GameObject projectile = ObjectLoader.LoadObject("Enemy Pellet", true);
        projectile.transform.position = _myContext.VisionTransform.position + (attackVector * 2);
        projectile.transform.rotation = Quaternion.LookRotation(attackVector);    
    }
}

