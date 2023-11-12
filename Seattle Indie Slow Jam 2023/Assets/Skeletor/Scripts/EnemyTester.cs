using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// Created By Skeletor
// simple script for the enemy test scene don't use this in the game
public class EnemyTester : MonoBehaviour
{
    // static reference to the player
    public static GameObject s_playerObject;
    public static Rigidbody s_playerPhysics;
    // public accessor for player
    public static GameObject PlayerObject => s_playerObject;
    [SerializeField] private float sensitivity = 300;
    private float xRotation;
    private float yRotation;
    private int previousGunCount;

    // finds the player on first frame
    void Awake()
    {
        s_playerObject = GameObject.FindGameObjectWithTag("Player");
        s_playerPhysics = s_playerObject.GetComponent<Rigidbody>();
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
        if(PlayerManager.instance.gunCount != previousGunCount)
        {
            MusicPlayer.CurrentLayer++;
        }
        previousGunCount = PlayerManager.instance.gunCount;
        /*
        if(Input.GetMouseButtonDown(0))
        {
            GameObject newPlayerProjectile = ObjectLoader.LoadObject("Player Projectile");
            newPlayerProjectile.transform.position = Camera.main.transform.position + (Camera.main.transform.forward * 2);
            newPlayerProjectile.transform.rotation = Camera.main.transform.rotation;
        }
        */
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
