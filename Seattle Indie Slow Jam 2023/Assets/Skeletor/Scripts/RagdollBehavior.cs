using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// attached to an object that can ragdoll
public class RagdollBehavior : MonoBehaviour
{   
    // used to only load components the first time the object is created
    private bool _loaded;
    // reference to each rigid body on the ragdoll
    private Rigidbody[] _bodies;
    // reference to each join on the ragdoll
    private CharacterJoint[] _joints;
    // reference to each collider on the ragdoll
    private Collider[] _colliders;
    // reference to center of the ragdoll for physics calculations
    [SerializeField] private Transform _root;

    // Start is called before the first frame update
    void Awake()
    {
        if(!_loaded)
        {
            _bodies = GetComponentsInChildren<Rigidbody>();
            _joints = GetComponentsInChildren<CharacterJoint>();
            _colliders = GetComponentsInChildren<Collider>();
            _loaded = true;
        }
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        EnableRagdoll(false);
    }

    // used to toggle ragdoll on and off, note that the animator component must be turned off separately in order to get the ragdoll to fall
    // 4 senarios
    // animator: enabled, ragdoll: enabled = AVOID! model animates normally. but still performs costly physics calculations
    // animator: enabled, ragdoll: disabled = model animates normally
    // animator: disabled, ragdoll: enabled = model will ragdoll with physics
    // animator: disabled, ragdoll disabled = model will freeze in current ragdoll position without doing costly physics animations 
    public void EnableRagdoll(bool value)
    {
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
            rigidBody.detectCollisions = value;
            rigidBody.useGravity = value;
        }
    }

    // applies ragdoll force from a source at a given magnitude to the ragdoll
    // source should be what ever is applying force, and magnitude should be the strength of that force
    public void ApplyRagdollForce(Vector3 source, float magnitude)
    {
        foreach(Rigidbody rigidBody  in _bodies)
        {
            rigidBody.AddExplosionForce(magnitude, source, 5);
        }
    }
}
