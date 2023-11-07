using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// helper behavior on an object that spawns a ragdoll
public class RagdollSpawner : MonoBehaviour
{
    // reference to the root
    [SerializeField] private Transform _root;
    [SerializeField] private GameObject _prefab;
    private string _prefabName;
    private Transform locations;
    private Transform[] _rigTransforms;

    void Awake()
    {
        _prefabName = _prefab.name;
        _rigTransforms = GetComponentsInChildren<Transform>();
    }

    public RagdollBehavior SpawnRagdoll()
    {
        RagdollBehavior ragdoll = ObjectLoader.LoadObject(_prefabName, true).GetComponent<RagdollBehavior>();
        ragdoll.MatchTransforms(_root, _rigTransforms);
        return ragdoll;
    }
}
