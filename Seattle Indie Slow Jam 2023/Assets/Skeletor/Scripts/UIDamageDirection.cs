using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// created by skeletor
// controls the hit indicator for the player when they are attacked
public class UIDamageDirection : MonoBehaviour
{
    [SerializeField] private GameObject _instance;

    // assign listener on start
    void Start()
    {
        PlayerManager.instance.OnPlayerInjured += OnPlayerInjured;
    }

    // called whenever the player is injured. Creates a damage indicator relative to the incoming hit that fades after a short time
    public void OnPlayerInjured(object caller, Vector3 hit)
    {
        Vector3 playerToDamagePosition = hit - PlayerManager.instance.transform.position; 
        float angle = Vector3.SignedAngle(PlayerManager.instance.transform.forward, playerToDamagePosition, Vector3.up);
        Debug.DrawRay(PlayerManager.instance.transform.position, playerToDamagePosition,  Color.green);
        Debug.DrawRay(PlayerManager.instance.transform.position, PlayerManager.instance.transform.forward, Color.blue);
        GameObject indicator = ObjectLoader.LoadObject(_instance.name, true);
        indicator.transform.SetParent(transform);
        indicator.transform.localPosition = Vector3.zero;
        indicator.transform.rotation = Quaternion.Euler(0,0,-angle);
    }
}
