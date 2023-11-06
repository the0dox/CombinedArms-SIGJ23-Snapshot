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
    // reference to the sprite visual of the projectile
    [SerializeField] private Transform _sprite;
    // time in seconds the projectile will travel before unloading
    [SerializeField] private float _maxRuntime;

    void Awake()
    {
        Invoke("Die", _maxRuntime);
    }

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
        Die();
        // see if the hit target can be attacked, if so deal damage to it
        other.TryGetComponent(out IAttackable target);
        target?.TakeDamage(gameObject, _damage);
    }

    // called when the projectile is destroyed
    public virtual void Die()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}
