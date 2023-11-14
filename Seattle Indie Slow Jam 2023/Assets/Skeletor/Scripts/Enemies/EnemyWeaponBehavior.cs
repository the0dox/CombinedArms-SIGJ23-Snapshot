using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by Skeletor
// this is the behavior of the weapon actively held by an enemy. THIS IS NOT TO BE CONFUSED WITH THE WEAPON DROP
public class EnemyWeaponBehavior : MonoBehaviour
{
    // public accessors
    public Transform LeftHandRigTarget => _leftHandRigTarget;
    public Transform RightHandRigTarget => _rightHandRigTarget;
    public RagdollSpawner MySpawner => _mySpawner;
    public State<EnemyBehavior> AttackState => EnemyStateFactory.BuildState(_attackState);
    public AudioClip SoundFX => _soundFX;

    // reference to the left arm attach point of the weapon
    [SerializeField] private Transform _leftHandRigTarget;
    // reference to the right arm attach point of the weapon
    [SerializeField] private Transform _rightHandRigTarget;
    // used to make a weapon drop when disabled
    [SerializeField] private RagdollSpawner _mySpawner;
    // matches a particular weapon type to an attack pattern
    [SerializeField] private StateKey _attackState;
    // the sound the enemy should play when firing this weapon type
    [SerializeField] private AudioClip _soundFX;

}
