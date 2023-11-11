using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GunData gun;

    private void OnTriggerEnter(Collider other)
    {
        if(gun != null) PlayerManager.instance.PickupGun(gun);
        gameObject.SetActive(false);
    }
}
