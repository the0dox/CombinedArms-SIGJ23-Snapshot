using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public GunData gun;
    public bool rotates = false;

    private void Update()
    {
        if (rotates == true)
        {
         transform.Rotate(0, -1f, 0);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(gun != null) PlayerManager.instance.PickupGun(gun);
        gameObject.SetActive(false);
    }
}
