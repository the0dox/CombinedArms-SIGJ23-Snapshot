using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float time = 10f;
    public int numberperspawn = 5;
    public int maxNum = 20;
    float timer = 0f;
    int num = 0;
    int waveNum = 0;
    bool waveAlive = false;
    static string[] EnemyNames = 
        {"KnifeHead",
        "Pistoleer",
        "Rifleman",
        "Rocketeer",
        "Shotgunner",
        "Sniper"};
      
    // Start is called before the first frame update
    void Start()
    {
        
    }
    //TODO: ENEMY RANDOMIZED SPAWNS
    public void OnKilled(object caller, System.EventArgs e)
    {
        num--;
        if (num < 1) waveAlive = false;
    }

    //WAVE SYSTEM
    // Update is called once per frame
    void Update()
    {
        if(!waveAlive) timer += Time.deltaTime;
        if (timer > time)
        {
            waveAlive = true;
            time = 0f;
            numberperspawn = Mathf.Clamp(numberperspawn + waveNum * 1, numberperspawn, maxNum);
        }
        if(waveAlive && num < 1)
        {
            for(int i = 0;i < numberperspawn; i++)
            {
                int j = Random.Range(0, EnemyNames.Length);
                GameObject g = ObjectLoader.LoadObject(EnemyNames[j]);
                g.transform.position = this.transform.position + new Vector3(
                    Random.Range(-3f,3f),0, Random.Range(-3f, 3f));
                num++;
            }
            waveAlive = true;
            timer = 0f;
        }
    }
}
