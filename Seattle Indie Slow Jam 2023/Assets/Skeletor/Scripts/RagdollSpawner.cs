using System.Collections;
using System.Collections.Generic;
using GameFilters;
using UnityEngine;

// created by skeletor
// not to be confused with the ragdoll behavior, which is attached to distinct ragdoll game object
// the ragdoll spawner is attached to the original object that creates and initalizes the ragdoll
// ensure that this component is attached to the model of the enemy and not the enemy itself for the most accurate ragdoll
public class RagdollSpawner : MonoBehaviour
{
    // reference to the main rigidbody of the object spawning the ragdoll, used to inherit the original velocity of the character
    [SerializeField] private Rigidbody _originalBody;
    // reference to the ragdoll prefab that this spawner will spawn. MAKE SURE THIS IS SET UP PROPERLY
    [SerializeField] private GameObject _prefab;
    // name of the ragdoll prefab
    private string _prefabName;
    // reference to all of the bones of the original model
    private Transform[] _rigTransforms;

    // when first initalized, assign a reference to all of my bones
    void Awake()
    {
        _prefabName = _prefab.name;
        _rigTransforms = gameObject.GetComponentsInChildrenByCondition((Transform bone) => bone.CompareTag(tag));
    }

    // called 
    public RagdollBehavior SpawnRagdoll()
    {
        Debug.Log("spawning ragdoll");
        RagdollBehavior ragdoll = ObjectLoader.LoadObject(_prefabName, true).GetComponent<RagdollBehavior>();
        ragdoll.MatchTransforms(transform, _rigTransforms, _originalBody);
        return ragdoll;
    }
}
