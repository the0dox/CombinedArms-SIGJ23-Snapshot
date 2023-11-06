using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by Skeletor
// the behavior for a simple enemy projectile
public class ProjectileBehavior : MonoBehaviour
{
    // the speed at which the projectile travels
    [SerializeField] private float _speed;
    // the damage the projectile deals
    [SerializeField] private float _damage;
    [SerializeField] private Transform _sprite;

    // on the physics update, move this projectile forward
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime, Space.Self);
        _sprite.LookAt(Camera.main.transform.position);
    }

    // Called whenever the projectile collides with somethin
    void OnTriggerEnter(Collider other)
    {
        // remove this projectile on collision
        gameObject.SetActive(false);
        // see if the hit target can be attacked, if so deal damage to it
        other.TryGetComponent(out IAttackable target);
        target?.TakeDamage(_damage);
    }
}
