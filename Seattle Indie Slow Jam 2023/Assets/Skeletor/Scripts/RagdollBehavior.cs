using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// attached to an object that can ragdoll
public class RagdollBehavior : MonoBehaviour, IAttackable
{   
    // reference to each rigid body on the ragdoll
    private Rigidbody[] _bodies;
    // reference to each join on the ragdoll
    private CharacterJoint[] _joints;
    // reference to each collider on the ragdoll
    private Collider[] _colliders;
    // the position and rotation values of each joint in the ragdoll
    private Transform[] _rigTransforms;
    [SerializeField] private Rigidbody _root; 
    // reference to center of the ragdoll for physics calculations
    private bool ragdollEnabled;
    private const float FREEZETIME = 20f;
    private readonly Vector3 _offset = new Vector3(0, 0.5f, 0);

    // Start is called before the first frame update
    void Awake()
    {
        _bodies = GetComponentsInChildren<Rigidbody>();
        _joints = GetComponentsInChildren<CharacterJoint>();
        _colliders = GetComponentsInChildren<Collider>();
        _rigTransforms = GetComponentsInChildren<Transform>();
    }

    void OnEnable()
    {
        EnableRagdoll(true);
        InvokeRepeating("Freeze", FREEZETIME, FREEZETIME);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    // used to toggle ragdoll on and off, note that the animator component must be turned off separately in order to get the ragdoll to fall
    // 4 senarios
    // animator: enabled, ragdoll: enabled = AVOID! model animates normally. but still performs costly physics calculations
    // animator: enabled, ragdoll: disabled = model animates normally
    // animator: disabled, ragdoll: enabled = model will ragdoll with physics
    // animator: disabled, ragdoll disabled = model will freeze in current ragdoll position without doing costly physics animations 
    private void EnableRagdoll(bool value)
    {
        ragdollEnabled = value;
        foreach(CharacterJoint joint in _joints)
        {
            joint.enableCollision = value;
        }
        foreach(Collider collider  in _colliders)
        {
            collider.enabled = value;
        }
        foreach(Rigidbody rigidBody  in _bodies)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.detectCollisions = value;
            rigidBody.useGravity = value;
            rigidBody.constraints = value ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
        }
    }

    // after a period of time check if I can freeze the rigidbody
    private void Freeze()
    {
        if(_bodies[0].velocity.magnitude < 0.05f)
        {
            if(ragdollEnabled)
            {
                EnableRagdoll(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    // WIP: Need to determine if it is worth it to make a new collider for the ragdoll just to detect player attacks
    public void TakeDamage(GameObject source, float damage)
    {
        ApplyRagdollForce(source.transform, 3000);
    }


    // applies ragdoll force from a source at a given magnitude to the ragdoll
    // source should be what ever is applying force, and magnitude should be the strength of that force
    public void ApplyRagdollForce(Transform source, float magnitude)
    {
        Vector3 hitLocation = source.position;
        Debug.DrawRay(hitLocation, Vector3.up, Color.green);
        Vector3 relativeAngle = _root.transform.position - hitLocation;
        relativeAngle = new Vector3(relativeAngle.x, 0.5f, relativeAngle.z);
        Debug.DrawRay(_root.transform.position, relativeAngle.normalized, Color.red);
        _root.velocity = relativeAngle.normalized * magnitude;
        foreach(Rigidbody body in _bodies)
        {
            body.AddExplosionForce(magnitude, source.position, 0.5f);
        }
        //Debug.DrawRay(_root.transform.position, _root.velocity, Color.red);
        //Debug.Break();
    }

    public void MatchTransforms(Transform root, Transform[] newTransforms)
    {
        transform.position = root.transform.position + _offset;
        transform.rotation = root.transform.rotation;
        if(_rigTransforms.Length != newTransforms.Length)
        {
            Debug.LogError($"new transforms do not match spawned ragdoll does Ragdoll spawner match the correct ragdoll? Expected: {_rigTransforms.Length} Got {newTransforms.Length}");
        }
        else
        {
            for(int i = 0; i < _rigTransforms.Length; i++)
            {
                _rigTransforms[i].transform.position = newTransforms[i].transform.position;
                _rigTransforms[i].transform.rotation = newTransforms[i].transform.rotation;
            }
        }
    }

}
