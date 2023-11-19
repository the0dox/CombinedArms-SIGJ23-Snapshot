using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// controls the behavior of a traveling rocket
[RequireComponent(typeof(Rigidbody))] 
public class MissleBehavior : MonoBehaviour
{
    // missle accelerates over time instead of traveling at a fixied speed 
    [SerializeField] private float _acceleration;
    // there is a maximum speed the missle can travel at any given time
    [SerializeField] private float _maximumSpeed;
    // how large the explosion is
    [SerializeField] private float _explosionRadius;
    // how much damage the explosion radius deals
    [SerializeField] private float _damage;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private AudioClip _explosionSound;
    private bool _isFriendly;

    // reference to the physics body attached to the object
    private Rigidbody _myBody;

    // assign components 
    void Awake()
    {
        _myBody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        _myBody.velocity = Vector3.zero;
        _isFriendly = true;
    }

    // apply speed as force every frame, while enforcing maximum speed
    void FixedUpdate()
    {
        _myBody.AddForce(transform.forward * _acceleration * (_myBody.velocity.magnitude < _maximumSpeed ? 1 : -1));
    } 

    // when the the rocket collides with something trigger an explosion!
    void OnTriggerEnter(Collider other)
    {
        if(_isFriendly)
        {
            if(other.tag.Equals("Player"))
            {
                return;
            }
        }
        else
        {
            if(other.tag.Equals("Enemy"))
            {
                return;
            }
        }
        gameObject.SetActive(false);
        GamePhysics.AttackSphereCast(transform.position, _explosionRadius, _damage);
        GameObject explosion = ObjectLoader.LoadObject(_explosionPrefab.name, true);
        explosion.transform.position = transform.position;
        AudioSource.PlayClipAtPoint(_explosionSound, transform.position);
    }

    public void SetHostile()
    {
        _isFriendly = false;
    }
}
