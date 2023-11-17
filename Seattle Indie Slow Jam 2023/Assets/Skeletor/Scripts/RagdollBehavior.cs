using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using GameFilters;
using Unity.VisualScripting;

// created by skeletor
// attached to an object that can ragdoll
public class RagdollBehavior : MonoBehaviour
{   
    // public accessor for the root of this ragdoll
    [SerializeField] private bool _despawn;
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
    // time it takes for the ragdoll to stop simulating phyiscs and eventually despawn
    private const float FREEZETIME = 20f;
    // velocity offset applied to the ragdoll to add
    private readonly Vector3 _offset = new Vector3(0, 0.5f, 0);

    // The first time this ragdoll is configured, assign all components
    void Awake()
    {
        _bodies = GetComponentsInChildren<Rigidbody>();
        _joints = GetComponentsInChildren<CharacterJoint>();
        _colliders = GetComponentsInChildren<Collider>();
        _rigTransforms = gameObject.GetComponentsInChildrenByCondition((Transform child) => child.tag.Equals(tag));
    }

    // called every time this ragdoll is spawned
    void OnEnable()
    {
        EnableRagdoll(true);
        if(_despawn)
            InvokeRepeating("Freeze", FREEZETIME, FREEZETIME);
    }

    // once this ragdoll is despawned it shouldn't check to despawn itself anymore
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

    // used when a ragdoll is swapped in for an animated model. 
    // copies the position, rotation, and velocity of the existing model.
    public void MatchTransforms(Transform root, Transform[] newTransforms, Rigidbody inhertitedBody)
    {
        transform.position = root.transform.position + _offset;
        transform.rotation = root.transform.rotation;
        if(_rigTransforms.Length != newTransforms.Length)
        {
            Debug.LogError($"new transforms do not match spawned ragdoll does Ragdoll spawner match the correct ragdoll? Expected: {_rigTransforms.Length} Got {newTransforms.Length}");
        }
        else
        {
            //StringBuilder debugString = new StringBuilder($"Assigning Ragdoll Transforms for {gameObject.name}\n");
            for(int i = 0; i < _rigTransforms.Length; i++)
            {
                //debugString.AppendFormat($"Matching Transform: ({_rigTransforms[i].name}) to Original: ({newTransforms[i].name}) position: {newTransforms[i].transform.position} rotation: ({newTransforms[i].transform.rotation})\n");
                _rigTransforms[i].transform.position = newTransforms[i].transform.position;
                _rigTransforms[i].transform.rotation = newTransforms[i].transform.rotation;
            }
            foreach(Rigidbody body in _bodies)
            {
                body.velocity = inhertitedBody.velocity;
            }
            //Debug.Log(debugString.ToString());
        }
    }

}
