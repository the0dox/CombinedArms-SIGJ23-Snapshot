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
    // reference to the point the weapon is attached to on the enemy model
    [SerializeField] private Transform _weaponAttachRoot;
    // reference to the rig constraint that matches the left hand to the weapon
    [SerializeField] private Transform _leftHandAttachPoint;
    // reference to the rig constraint that matches the right hand to the weapon
    [SerializeField] private Transform _rightHandAttachPoint;
    // reference to the rigid body of the enemy that is passed onto the weapon
    [SerializeField] private Rigidbody _originalBody;
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
        _leftHandAttachPoint.transform.localPosition = _activeWeapon.LeftHandRigTarget.transform.localPosition;
        _leftHandAttachPoint.transform.localRotation = _activeWeapon.LeftHandRigTarget.transform.localRotation;
        _rightHandAttachPoint.transform.localPosition = _activeWeapon.RightHandRigTarget.transform.localPosition;
        _rightHandAttachPoint.transform.localRotation = _activeWeapon.RightHandRigTarget.transform.localRotation;
    }

    // unassigns weapon and drops a weapon where it was
    public void DropWeapon(float spawnRate = 1)
    {
        if(_activeWeapon != null)
        {
            if(Random.Range(0,1f) <= spawnRate)
            {
                _activeWeapon.MySpawner.SpawnRagdoll();
            }
            _activeWeapon.transform.SetParent(null);
            _activeWeapon.gameObject.SetActive(false);    
        }
        _activeWeapon = null;
    }
}
