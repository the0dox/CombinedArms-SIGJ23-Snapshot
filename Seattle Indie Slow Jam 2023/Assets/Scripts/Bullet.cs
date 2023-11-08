using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float dmg = 1f;
    public Vector3 dir;
    public float speed = 1f;

    private void Update()
    {
        this.transform.position += dir.normalized * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.transform.gameObject.SetActive(false);
        //Destroy(this.gameObject);    
    }
}
