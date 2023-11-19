using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IAttackable damagecomponent))
        {
            damagecomponent.TakeDamage(other.transform.position, 999);
        }
    }
}
