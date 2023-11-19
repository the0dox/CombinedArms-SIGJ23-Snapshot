using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour, IAttackable
{
    public static PlayerManager instance;
    [SerializeField]
    GunData[] gunDatas;
    [SerializeField]
    AudioSource eatSource;
    public float health = 100f;
    float currentHealth;
    public int gunCount = 0;
    public float healthInc = 5f;
    public float eatLimit = 2f;
    [SerializeField]
    AudioClip munchClip;
    [HideInInspector]
    public int gunsReloading = 0;
    Vector4 gunPlacementRange = new Vector4(-1, 1, -.8f, .8f);
    Vector2 placeOffsetAmount = Vector2.zero;
    Vector2 maxPlaceOffset;
    Dictionary<GameObject, int> prefabCount;
    Dictionary<GameObject, GameObject> parentMap;
    List<GameObject> gunList;
    int nextPlacement;
    private float eatTimer;

    private Rigidbody physicsBody;
    public static Vector3 CurrentVelocity => instance.physicsBody.velocity;
    private FirstPersonController PlayerMovement;
    public static bool CameraCanMove{set => instance.PlayerMovement.cameraCanMove = value;}
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
        physicsBody = GetComponent<Rigidbody>();
        PlayerMovement = GetComponent<FirstPersonController>();
        PlayerMovement.DashStarted += OnDashStarted;
        PlayerMovement.DashEnded += OnDashEnded;
        PlayerMovement.JumpStarted += OnJumpStarted;
        placeOffsetAmount.x = gunPlacementRange.y - gunPlacementRange.x;
        placeOffsetAmount.y = gunPlacementRange.w - gunPlacementRange.z;
        maxPlaceOffset = placeOffsetAmount;
        prefabCount = new Dictionary<GameObject, int>();
        parentMap = new Dictionary<GameObject, GameObject>();
        gunList = new List<GameObject>();
        nextPlacement = 0; //Next index in the gun array
        eatSource.pitch = eatLimit * .64f;
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
        if(OnHealthChanged != null)
        {
            OnHealthChanged(this, currentHealth/health);
        }
        _SFXSource.PlayOneShot(munchClip);
        OnGunCountChanged();
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
        OnGunCountChanged();
    }

    void OnGunCountChanged()
    {
        int baseLayer = 2;
        baseLayer = Mathf.Clamp(baseLayer + (gunCount/3), baseLayer, 5);
        Debug.Log("setting current layer to " + baseLayer + " because I have " + gunCount + " guns");
        MusicPlayer.CurrentLayer = baseLayer;
    }


    void PickupGun()
    {
        int r = Random.Range(0, gunDatas.Length);
        GunData data = gunDatas[r];
        //data.RandomizeProperties();
        PickupGun(data);
        OnGunCountChanged();
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    PickupGun();
        //}
        if (Input.GetKey(KeyCode.Mouse1) && gunCount > 1 && gunsReloading < 1)
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
        if(!_dashInvulnerable && !GameManager.GameHasEnded)
        {
            currentHealth -= value;
            _SFXSource.PlayOneShot(_hurtSFX);
            if(OnPlayerInjured != null)
            {
                OnPlayerInjured(this, hit);
            }
            if(OnHealthChanged != null)
            {
                OnHealthChanged(this, currentHealth/health);
            }
            
            if (currentHealth <= 0)
            {
                PlayerMovement.playerCanMove = false;
                PlayerMovement.cameraCanMove = false;
                _SFXSource.PlayOneShot(_deathSFX);
                GameManager.TriggerGameOver();
            }
        }
    }

    # region skeletor
    // is set to true the player cannot take damage
    private bool _dashInvulnerable;
    // the number of frames the player is invulnerable 
    [SerializeField] private int _dashFrames;
    // reference to the particles that appear when dashing
    [SerializeField] private ParticleSystem _dashParticle;
    // reference to sound that plays when dashing
    [SerializeField] private AudioClip _dashSFX;
    // reference to the sound that plays when jumping
    [SerializeField] private AudioClip _jumpSFX;
    // sound played when injured
    [SerializeField] private AudioClip _hurtSFX;
    // sound played when killed
    [SerializeField] private AudioClip _deathSFX;
    // source of sound effects that aren't distorted when eating
    [SerializeField] private AudioSource _SFXSource;
    // event raised when injured
    public EventHandler<Vector3> OnPlayerInjured;
    // event raised when injured
    public EventHandler<float> OnHealthChanged;

    // called when player movement starts a dash
    public void OnDashStarted(object caller, EventArgs e)
    {
        Vector3 particleDirection = CurrentVelocity;
        particleDirection = new Vector3(particleDirection.x, 0, particleDirection.z);
        particleDirection = particleDirection.magnitude > 0.05f ? particleDirection : transform.forward;
        _dashParticle.transform.rotation = Quaternion.LookRotation(particleDirection);
        _dashParticle.Play();
        _SFXSource.PlayOneShot(_dashSFX, 1);
        StartCoroutine(DashDelay());
    }

    // called when player movement starts a jump
    public void OnJumpStarted(object caller, EventArgs e)
    {
        _SFXSource.PlayOneShot(_jumpSFX);
    }

    // called when player ends a dash
    public void OnDashEnded(object caller, EventArgs e)
    {
        _dashParticle.Stop();
    }

    // while active, the player cannot take damage
    private IEnumerator DashDelay()
    {
        _dashInvulnerable = true;
        for(int i = 0; i < _dashFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        _dashInvulnerable = false;
    }

    #endregion
}
