using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// simple billboard effect to look at the main camera
public class LookAtCamera : MonoBehaviour
{

    // Update is called once per frame
    void LateUpdate()
    {
        gameObject.transform.LookAt(Camera.main.transform.position);
    }
}
