using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is a helper class for all physics calculations for ragdolls and weapon damage
// Controls the amount of bounce ragdolls will recieve from attacks
// these modifiers are fully configureable feel free to adjust to your liking
public static class GamePhysics
{       
    // amount of force applied per point of damage dealt
    private static readonly float MAGNITUDEMODIFIER = 10;
    // the maximum force magnitude that can be applied from a single attack
    private static readonly float MAXMAGNITUDE = 75;
    // the base y offset applied to the force vector. The higher the y offset, the higher up the body will be pushed
    private static readonly float FORCEYOFFSET = 0.2f;
    // the amount of extra y offset applied per point of damage dealt. 
    private static readonly float YMAGNITUDEMODIFIER = 0.05f;
    // the maximum y offset that can be applied from a single attack
    private static readonly float YMAXNITUDEMAX = 0.6f;
    // determines the point at which a rigidbody is moving too fast to safetly recieve any additional force
    private static readonly float MAXIMUMANGULARVELOCITY = 3f;

    // applies force to a rigid body from a point origin and a given amount of damage
    private static void ApplyDamageForce(this Rigidbody affectedBody, Vector3 damageOrigin, float damage)
    {
        // only applies force if the bodies angular velocity is under the cap
        if(affectedBody.angularVelocity.magnitude < MAXIMUMANGULARVELOCITY)
        {
            Vector3 relativeAngle = affectedBody.transform.position - damageOrigin;
            relativeAngle = new Vector3(relativeAngle.x, 0, relativeAngle.z).normalized + new Vector3(0, FORCEYOFFSET + Mathf.Clamp(YMAGNITUDEMODIFIER * damage, 0, YMAXNITUDEMAX), 0);
            affectedBody.AddForce(relativeAngle * Mathf.Clamp(damage * MAGNITUDEMODIFIER, 0, MAXMAGNITUDE), ForceMode.Impulse);
        }
        else
        {
            Debug.LogFormat($"angular velocity on {affectedBody.name} excedees max, preventing force from being applied");
        }
   }

    // this is the method that should be used by raycast weapons to attack targets
    // returns true if the raycast hit something that can be attacked
    // attackRay: the normalized directional ray of the weapon attack
    // damage: the damage value of this attack, used to determine both how much health is subtracted and how far the enemy is pushed
    // range: the maximum range of the weapon cast
    // hitInfo: the output of the attackraycast will contain a reference to the enemy collider if there was a hit
    public static bool AttackRayCast(Ray attackRay, float damage, float range, out RaycastHit hitInfo)
    {
        if(Physics.Raycast(attackRay,hitInfo: out RaycastHit hit,  maxDistance: range, layerMask: LayerMask.GetMask("QueryAttack")))
        {
            hitInfo = hit;
            // check a second time if line of sight is blocked
            if(Physics.Raycast(attackRay, hit.distance, LayerMask.GetMask("Terrain")))
            {
                return false;
            }
            if(hit.collider.TryGetComponent(out Rigidbody body))
            {
                body.ApplyDamageForce(hit.point, damage);
            }
            if(hit.collider.TryGetComponent(out IAttackable damageTarget))
            {
                damageTarget.TakeDamage(hit.point, damage);
            }
            return true;
        }
        hitInfo = hit;
        return false;
    }
}
