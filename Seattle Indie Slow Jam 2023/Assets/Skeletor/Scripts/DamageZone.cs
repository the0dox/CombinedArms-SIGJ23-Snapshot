using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// created by skeletor
// a persistent damage zone that hurts a player when they first enter it
public class DamageZone : MonoBehaviour
{
    // the damage the projectile deals
    [SerializeField] private float _damage;

    // Called whenever the projectile collides with somethin
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger entered");
        // see if the hit target can be attacked, if so deal damage to it
        if (other.TryGetComponent(out IAttackable target))
        {
            target.TakeDamage(transform.position, _damage);
            OnDamageTriggered(target);
        }
        OnCollisionTriggered(other);
    } 

    // called whenever this damage trigger is entered
    protected virtual void OnDamageTriggered(IAttackable target){}

    // called whenever this damage trigger is entered
    protected virtual void OnCollisionTriggered(Collider other){}
}
