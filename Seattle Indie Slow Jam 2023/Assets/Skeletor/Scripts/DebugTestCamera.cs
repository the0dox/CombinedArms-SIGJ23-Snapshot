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
        Cursor.lockState = CursorLockMode.Confined;
        // shouldn't be allowed in built versions of the game
        #if !UNITY_EDITOR
            Destroy(this);
        #endif
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            TestShoot();
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
        if(GUILayout.Button("SpawnEnemy"))
        {
            ObjectLoader.LoadObject("Enemy");
        }
        else if(GUILayout.Button("Spawn Projectile"))
        {
            TestShoot();
        }
    }

    void TestShoot()
    {
        GameObject testProjectile = ObjectLoader.LoadObject("Player Projectile");
        testProjectile.transform.position = transform.position + transform.forward;
        testProjectile.transform.eulerAngles = transform.eulerAngles;
    }
}
