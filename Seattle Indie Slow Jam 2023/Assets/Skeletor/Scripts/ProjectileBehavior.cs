using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// created by Skeletor
// the behavior for a simple enemy projectile
public class ProjectileBehavior : DamageZone
{
    // the speed at which the projectile travels
    [SerializeField] private float _speed;
    // reference to the sprite visual of the projectile
    [SerializeField] private Transform _sprite;
    // time in seconds the projectile will travel before unloading
    [SerializeField] private float _maxRuntime;

    void OnEnable()
    {
        Invoke("Die", _maxRuntime);
    }

    // on the physics update, move this projectile forward
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime, Space.Self);
        _sprite.LookAt(Camera.main.transform.position);
    }

    // called when the projectile is destroyed
    protected override void OnCollisionTriggered(Collider other)
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}
