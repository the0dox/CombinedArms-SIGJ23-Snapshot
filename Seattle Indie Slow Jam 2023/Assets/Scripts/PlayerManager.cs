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
    public float health = 100f;
    float currentHealth;
    public int gunCount = 0;
    [HideInInspector]
    public int gunsReloading = 0;
    Vector4 gunPlacementRange = new Vector4(-1, 1, -.8f, .8f);
    Vector2 placeOffsetAmount = Vector2.zero;
    float timer = 0f;
    float timeLimit = 1f;
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
    }

    public void PickupGun(GunData data)
    {
        GameObject gun = Instantiate(data.gunPrefab, this.transform.GetChild(0).GetChild(0));
        gun.GetComponent<Gun>().Initalize();
        gun.GetComponent<Gun>().ApplyGunData(data);
        gun.GetComponent<Animator>().enabled = false;
        Vector3 startPos = new Vector3(gunPlacementRange.y, gunPlacementRange.z, .7f);
        if (gunCount < 1)
        {
            gun.transform.localPosition = startPos;
            gunCount++;
        }
        else
        {
            int dir = gunCount % 4;
            int on = (int)(gunCount / 4);
            on = Mathf.Clamp(on, 0, 1);
            switch (dir)
            {
                case 0:
                    gun.transform.localPosition = new Vector3(gunPlacementRange.y, gunPlacementRange.z, .7f);
                    placeOffsetAmount /= 1.5f;//2f;
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
    }

    public void TakeDamage(Vector3 hit, float value)
    {
        currentHealth -= value;
        slider.value = currentHealth / health;
        Vector3 v = Camera.main.WorldToScreenPoint(hit);
        dmgDirection.transform.position = v;
        dmgDirection.SetActive(true);
        timer = 0;
        if (currentHealth <= 0) deathUI.SetActive(true);
    }
}
