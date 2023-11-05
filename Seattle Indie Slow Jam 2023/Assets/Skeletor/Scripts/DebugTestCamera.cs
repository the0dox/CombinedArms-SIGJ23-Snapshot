using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Created By Skeletor
// simple script for the enemy test scene
public class DebugTestCamera : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    #if UNITY_EDITOR
    void OnGUI() {
        if (GUILayout.Button("First Person")){  
            
        }
    }
    #endif
}
