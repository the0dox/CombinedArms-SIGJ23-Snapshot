using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// created by skeletor
// helper behavior that spawns a weapon on an enemy when the enemy is first created
public class EnemyLoadoutBehavior : MonoBehaviour
{
    // public accessor
    public EnemyWeaponBehavior ActiveWeapon => _activeWeapon;
    // reference to the rig constraint that matches the left hand to the weapon
    [SerializeField] private TwoBoneIKConstraint _leftHandIK;
    // reference to the rig constraint that matches the right hand to the weapon
    [SerializeField] private TwoBoneIKConstraint _rightHandIK;
    // reference to the point the weapon is attached to on the enemy model
    [SerializeField] private Transform _weaponAttachRoot;
    // reference to the rigid body of the enemy that is passed onto the weapon
    [SerializeField] private Rigidbody _originalBody;
    // primary controller of the IK components
    [SerializeField] private RigBuilder _rig;
    // reference to the weapon the enemy is currently holding
    private EnemyWeaponBehavior _activeWeapon;

    // creates an enemy weapon and attaches it to the enemy model
    public void EquipWeapon(GameObject weapon)
    {
        _activeWeapon = ObjectLoader.LoadObject(weapon.name).GetComponent<EnemyWeaponBehavior>();
        _activeWeapon.transform.SetParent(_weaponAttachRoot);
        _activeWeapon.transform.localPosition = Vector3.zero;
        _activeWeapon.transform.localRotation = Quaternion.identity;
        _activeWeapon.MySpawner.OriginalBody = _originalBody;
        _leftHandIK.data.target = _activeWeapon.LeftHandRigTarget;
        _rightHandIK.data.target = _activeWeapon.RightHandRigTarget;
        _rig.Build();
    }

    // unassigns weapon and drops a weapon where it was
    public void DropWeapon()
    {
        if(_activeWeapon != null)
        {
            _activeWeapon.MySpawner.SpawnRagdoll();
            _activeWeapon.transform.SetParent(null);
            _activeWeapon.gameObject.SetActive(false);    
        }
        _activeWeapon = null;
        _leftHandIK.data.target = null;
        _leftHandIK.data.target = null;
    }
}
