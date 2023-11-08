using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IAttackable
{
    public static PlayerManager instance;
    [SerializeField]
    GameObject gunPrefab;
    public float health = 100f;
    public int gunCount = 0;
    [HideInInspector]
    public int gunsReloading = 0;
    Vector4 gunPlacementRange = new Vector4(-1, 1, -.8f, .8f);
    Vector2 placeOffsetAmount = Vector2.zero;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        placeOffsetAmount.x = gunPlacementRange.y - gunPlacementRange.x;
        placeOffsetAmount.y = gunPlacementRange.w - gunPlacementRange.z;
    }

    void PickupGun()
    {
        GunData data = ScriptableObject.CreateInstance<GunData>();
        data.RandomizeProperties();
        GameObject gun = Instantiate(gunPrefab, this.transform.GetChild(0).GetChild(0));
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
                    break;
                case 1:
                    gun.transform.localPosition = new Vector3(gunPlacementRange.x, gunPlacementRange.z, .7f);
                    gun.transform.localPosition += on * new Vector3(0, placeOffsetAmount.y, 0);
                    break;
                case 2:
                    gun.transform.localPosition = new Vector3(gunPlacementRange.x, gunPlacementRange.w, .7f);
                    gun.transform.localPosition += on * new Vector3(placeOffsetAmount.x, 0, 0);
                    break;
                case 3:
                    gun.transform.localPosition = new Vector3(gunPlacementRange.y, gunPlacementRange.w, .7f);
                    gun.transform.localPosition -= on * new Vector3(0, placeOffsetAmount.y, 0);
                    break;
            }
            gunCount++;
        }
        gun.GetComponent<Animator>().enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PickupGun();
        }
    }

    public void TakeDamage(Vector3 hit, float value)
    {
        health -= value;
        if (health < 0) Debug.Log("I AM DEAD! NOOOOOOO!");
    }
}
