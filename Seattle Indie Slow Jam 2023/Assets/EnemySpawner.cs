using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float time = 10f;
    public int numberperspawn = 5;
    public int maxNum = 20;
    public int enemyRangeWave = 3;
    public int dropWaves = 2;
    public int enemyIncreaseWaves = 2;
    public int endArenaWave = 2; //for testing
    [SerializeField]
    GameObject[] weaponDrops;
    [SerializeField]
    GameObject endTrigger;
    float timer = 0f;
    int num = 0;
    int dropNum = 0;
    int waveNum = 0;
    int enemyRange = 1;
    bool waveAlive = false;
    static string[] EnemyNames = 
        {"KnifeHead",
        "Pistoleer",
        "Shotgunner",
        "Rifleman",
        "Rocketeer",
        "Sniper"};
      
    // Start is called before the first frame update
    void Start()
    {
        endTrigger.SetActive(false);
    }
    //TODO: ENEMY RANDOMIZED SPAWNS
    public void OnKilled(object caller, System.EventArgs e)
    {
        num--;
        if (num < 1 && waveAlive)
        {
            waveAlive = false;
            waveNum++;
            Debug.Log("Waves: " + waveNum);
            if (waveNum >= endArenaWave)
            {
                endTrigger.SetActive(true);
                return;
            }
            if (waveNum % enemyRangeWave == 0) enemyRange = Mathf.Clamp(enemyRange + 1, 0, EnemyNames.Length);
            if(waveNum % dropWaves == 0)
            {
                int dropMax = Mathf.Clamp(dropNum, 0, weaponDrops.Length);
                GameObject g = GameObject.Instantiate(weaponDrops[dropMax]);
                g.transform.position = transform.localPosition;//new Vector3(0, 0, 0);
                dropNum = dropMax;
                dropNum++;
            }
        }
    }

    //WAVE SYSTEM
    // Update is called once per frame
    void Update()
    {
        if(!waveAlive) timer += Time.deltaTime;
        if (timer > time)
        {
            waveAlive = true;
            timer = 0f;
            numberperspawn = Mathf.Clamp(numberperspawn + waveNum /enemyIncreaseWaves, numberperspawn, maxNum);
        }
        if(waveAlive && num < 1)
        {
            for(int i = 0;i < numberperspawn; i++)
            {
                int j = Random.Range(0, enemyRange);
                GameObject g = ObjectLoader.LoadObject(EnemyNames[j]);
                g.transform.position = this.transform.position + new Vector3(
                    Random.Range(-3f,3f),0, Random.Range(-3f, 3f));
                g.GetComponent<EnemyBehavior>().ArenaModeSetup();
                g.GetComponent<EnemyBehavior>().Killed += OnKilled;
                num++;
            }
            waveAlive = true;
            timer = 0f;
        }
    }
}
