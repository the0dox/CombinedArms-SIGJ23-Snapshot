using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float time = 10f;
    public int numberperspawn = 5;
    float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > time)
        {
            for(int i = 0;i < numberperspawn; i++)
            {
                ObjectLoader.LoadObject("Enemy").transform.position = this.transform.position + new Vector3(
                    Random.Range(-3f,3f),0, Random.Range(-3f, 3f));
            }
            timer = 0f;
        }
    }
}
