using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// Created By Skeletor
// simple script for the enemy test scene don't use this in the game
public class DebugTestCamera : MonoBehaviour
{
    // static reference to the player
    public static GameObject s_playerObject;
    // public accessor for player
    public static GameObject PlayerObject => s_playerObject;
    [SerializeField] private float sensitivity = 300;
    private float xRotation;
    private float yRotation;

    // finds the player on first frame
    void Awake()
    {
        s_playerObject = GameObject.FindGameObjectWithTag("Player");
        Cursor.lockState = CursorLockMode.Locked;
        // shouldn't be allowed in built versions of the game
        #if !UNITY_EDITOR
            Destroy(this);
        #endif
    }
    
    void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            ObjectLoader.LoadObject("Enemy").transform.position = new Vector3(0,0,5);
        }
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;;
        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.rotation =  Quaternion.Euler(xRotation, yRotation, 0);
    }

    void OnGUI()
    {
        if(GUILayout.Button("E: SpawnEnemy"))
        {
            ObjectLoader.LoadObject("Enemy").transform.position = new Vector3(0,0,5);
        }
    }

}
