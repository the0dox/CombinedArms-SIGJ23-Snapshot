using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference to the weapon held by this turret
public class ArmTurretBehavior : MonoBehaviour
{
    [SerializeField] private Transform _weaponRoot;
    public void Update()
    {
        _weaponRoot.LookAt(PlayerManager.instance.transform.position);
    }
}
