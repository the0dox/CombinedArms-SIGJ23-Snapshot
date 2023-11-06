using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Created By Skeletor
// simple script for the enemy test scene don't use this in the game
public class DebugTestCamera : MonoBehaviour
{
    // static reference to the player
    public static GameObject s_playerObject;
    // public accessor for player
    public static GameObject PlayerObject => s_playerObject;

    // finds the player on first frame
    void Awake()
    {
        s_playerObject = GameObject.FindGameObjectWithTag("Player");
        // shouldn't be allowed in built versions of the game
        #if !UNITY_EDITOR
            Destroy(this);
        #endif
    }

    void OnGUI()
    {
        if(GUILayout.Button("SpawnEnemy"))
        {
            ObjectLoader.LoadObject("Enemy");
        }
        else if(GUILayout.Button("Spawn Projectile"))
        {
            GameObject testProjectile = ObjectLoader.LoadObject("Projectile");
            testProjectile.transform.position = transform.position;
            testProjectile.transform.eulerAngles = transform.eulerAngles;
        }
    }
}
