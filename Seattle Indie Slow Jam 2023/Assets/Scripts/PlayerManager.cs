using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IAttackable
{
    public static PlayerManager instance;
    [SerializeField]
    GunData[] gunDatas;
    [SerializeField]
    GameObject deathUI;
    [SerializeField]
    GameObject dmgDirection;
    [SerializeField]
    Slider slider;
    [SerializeField]
    AudioSource eatSource;
    public float health = 100f;
    float currentHealth;
    public int gunCount = 0;
    public float healthInc = 5f;
    public float eatLimit = 2f;
    [HideInInspector]
    public int gunsReloading = 0;
    Vector4 gunPlacementRange = new Vector4(-1, 1, -.8f, .8f);
    Vector2 placeOffsetAmount = Vector2.zero;
    Vector2 maxPlaceOffset;
    float timer = 0f;
    float timeLimit = 1f;
    Dictionary<GameObject, int> prefabCount;
    Dictionary<GameObject, GameObject> parentMap;
    List<GameObject> gunList;
    int nextPlacement;
    private float eatTimer;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
        placeOffsetAmount.x = gunPlacementRange.y - gunPlacementRange.x;
        placeOffsetAmount.y = gunPlacementRange.w - gunPlacementRange.z;
        maxPlaceOffset = placeOffsetAmount;
        prefabCount = new Dictionary<GameObject, int>();
        parentMap = new Dictionary<GameObject, GameObject>();
        gunList = new List<GameObject>();
        nextPlacement = 0; //Next index in the gun array
        eatSource.pitch = eatLimit * .16f;
    }

    void EatGun()
    {
        if (gunCount < 2) return;
        int max = -999;
        GameObject p = null;
        foreach(KeyValuePair<GameObject,int> k in prefabCount)
        {
            if( k.Value > max)
            {
                max = k.Value;
                p = k.Key;
            }
        }
        if (p == null) return;
        List<int> SelctedPrefabIndices = new List<int>();
        for(int i = 0;i < gunList.Count;i++)
        {
            if (gunList[i] != null && parentMap[gunList[i]].Equals(p))
            {
                SelctedPrefabIndices.Add(i);                   
            }
        }
        GameObject g = gunList[SelctedPrefabIndices[Random.Range(0, SelctedPrefabIndices.Count)]];
        gunCount--;
        prefabCount[parentMap[g]]--;
        parentMap.Remove(g);
        int index = gunList.IndexOf(g);
        nextPlacement =  index < nextPlacement ? index : nextPlacement;
        gunList[index] = null;
        Destroy(g);
        currentHealth = Mathf.Clamp(currentHealth + healthInc, currentHealth, health);
        slider.value = currentHealth / health;
    }
    public void PickupGun(GunData data)
    {
        if (prefabCount.ContainsKey(data.gunPrefab))
        {
            prefabCount[data.gunPrefab]++;
        }else prefabCount[data.gunPrefab] = 1;

        GameObject gun = Instantiate(data.gunPrefab, this.transform.GetChild(0).GetChild(0));
        parentMap[gun] = data.gunPrefab;
        gun.GetComponent<Gun>().Initalize();
        gun.GetComponent<Gun>().ApplyGunData(data);
        gun.GetComponent<Animator>().enabled = false;
        Vector3 startPos = new Vector3(gunPlacementRange.y, gunPlacementRange.z, .7f);
        //int nextPlacement = placeStack.Pop();
        if (gunCount < 1)
        {
            gun.transform.localPosition = startPos;
            gunCount++;
        }
        else
        {
            int dir = (nextPlacement+1) % 4;//Accounting for the fact we start with an unlisted gun
            int on = (int)(gunCount / 4);
            on = Mathf.Clamp(on, 0, 1);
            switch (dir)
            {
                case 0:
                    gun.transform.localPosition = new Vector3(gunPlacementRange.y, gunPlacementRange.z, .7f);
                    placeOffsetAmount = maxPlaceOffset / (Mathf.Floor((nextPlacement+1)/4)*1.5f);//2f;
                    gun.transform.localPosition -= on * new Vector3(placeOffsetAmount.x, 0, 0);
                    if (gun.GetComponent<Gun>().UsesUI)
                    {
                        gun.GetComponent<Gun>().ammoUI.transform.localPosition = new Vector3(-0.2f,.2f, 0.5f);
                        gun.GetComponent<Gun>().ammoUI.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    }
                    break;
                case 1:
                    gun.transform.localPosition = new Vector3(gunPlacementRange.x, gunPlacementRange.z, .7f);
                    gun.transform.localPosition += on * new Vector3(0, placeOffsetAmount.y, 0);
                    if (gun.GetComponent<Gun>().UsesUI)
                    {
                        gun.GetComponent<Gun>().ammoUI.transform.localPosition = new Vector3(0.2f, 0.2f, 0.5f);
                        gun.GetComponent<Gun>().ammoUI.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                    }
                    break;
                case 2:
                    gun.transform.localPosition = new Vector3(gunPlacementRange.x, gunPlacementRange.w, .7f);
                    gun.transform.localPosition += on * new Vector3(placeOffsetAmount.x, 0, 0);
                    if (gun.GetComponent<Gun>().UsesUI)
                    {
                        gun.GetComponent<Gun>().ammoUI.transform.localPosition = new Vector3(0.2f, 0.2f,0.5f);
                        gun.GetComponent<Gun>().ammoUI.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                    }
                    break;
                case 3:
                    gun.transform.localPosition = new Vector3(gunPlacementRange.y, gunPlacementRange.w, .7f);
                    gun.transform.localPosition -= on * new Vector3(0, placeOffsetAmount.y, 0);
                    if (gun.GetComponent<Gun>().UsesUI)
                    {
                        gun.GetComponent<Gun>().ammoUI.transform.localPosition = new Vector3(-0.2f,0.2f, 0.5f);
                        gun.GetComponent<Gun>().ammoUI.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    }
                    break;
            }
            gunCount++;
            if(nextPlacement >= gunList.Count)
            {
                gunList.Add(gun);
                nextPlacement = gunList.Count;
            }else
            {
                gunList[nextPlacement] = gun;
                //Find the next empty space
                //We know the there isn't an empty space behind the space we just filled, so look ahead.
                int result = -1;
                for(int n = nextPlacement;n < gunList.Count; n++)
                {
                    if(gunList[n] == null)
                    {
                        result = n;
                        break;
                    }
                }
                if (result < 0) nextPlacement = gunList.Count;
                else nextPlacement = result;
            }
        }
        gun.GetComponent<Animator>().enabled = true;
    }
    void PickupGun()
    {
        int r = Random.Range(0, gunDatas.Length);
        GunData data = gunDatas[r];
        data.RandomizeProperties();
        PickupGun(data);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PickupGun();
        }
        timer += Time.deltaTime;
        if(timer > timeLimit)
        {
            dmgDirection.SetActive(false);
            timer = 0;
        }
        if (Input.GetKey(KeyCode.F) && gunCount > 1)
        {
            eatTimer += Time.deltaTime;
            Debug.Log("Eat in: " + eatTimer);
            if(!eatSource.isPlaying) eatSource.Play();
            if (eatTimer > eatLimit) {
                EatGun();
                eatTimer = 0f;
            }
        }
        else
        {
            if(eatSource.isPlaying) eatSource.Stop();
            eatTimer = 0f;
        }
    }

    public void TakeDamage(Vector3 hit, float value)
    {
        currentHealth -= value;
        slider.value = currentHealth / health;
        Vector3 v = Camera.main.WorldToScreenPoint(hit);
        dmgDirection.transform.position = v;
        dmgDirection.SetActive(true);
        timer = 0;
        if (currentHealth <= 0)
        {
            deathUI.SetActive(true);
            FindObjectOfType<GameManager>().EndGame();
        }
    }
}
