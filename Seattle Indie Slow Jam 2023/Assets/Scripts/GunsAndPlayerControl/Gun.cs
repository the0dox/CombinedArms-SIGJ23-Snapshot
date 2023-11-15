using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //ScriptibleObject here -->>>
    public float firerate = 0.5f;
    public float dmg = 1f;
    public float reloadSpeed = 1f;
    public float ammoTotal = 100f;
    public float ammoPerBullet = 1f;
    public float range = 10f;
    public float weaponSpread = 0f;
    public int numOfBullets = 1;
    public bool isAuto = true;
    public AnimationClip fireAni;
    public AnimationClip reloadAni;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip clickSound;
    public GameObject bullet;
    public TrailRenderer hitscanBullet;
    public bool UsesUI = false;
    public GameObject ammoUI;
    public AudioSource barrel;
    float ammoCount;
    Animator animator;
    MeshRenderer renderer;
    bool isReloading = false;
    bool isFiring = false;
    bool hasFired = false;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        renderer = this.GetComponent<MeshRenderer>();
        barrel = this.transform.GetChild(0).GetComponent<AudioSource>();
        animator.SetFloat("fireSpeed", firerate);
        animator.SetFloat("reloadSpeed", reloadSpeed);
        ammoCount = ammoTotal;
        foreach(AnimationClip a in animator.runtimeAnimatorController.animationClips)
        {

            if (a.name.Contains("Reload") && a.events.Length < 1)
            {
                AnimationEvent e = new AnimationEvent();
                e.time = a.length;
                e.functionName = "ReloadWeapon";
                a.AddEvent(e);
            }
            else if (a.name.Contains("Fire") && a.events.Length < 1)
            {
                AnimationEvent e = new AnimationEvent();
                e.time = 0;
                e.functionName = "FireWeapon";
                a.AddEvent(e);
            }
            startPos = this.transform.localPosition;
        }
        ammoUI.GetComponent<TMP_Text>().text = ammoCount.ToString();
    }
    public void Initalize()
    {
        animator = this.GetComponent<Animator>();
        renderer = this.GetComponent<MeshRenderer>();
        barrel = this.transform.GetChild(0).GetComponent<AudioSource>();
        animator.SetFloat("fireSpeed", firerate);
        animator.SetFloat("reloadSpeed", reloadSpeed);
        ammoCount = ammoTotal;
        startPos = this.transform.localPosition;
        /*
        foreach (AnimationClip a in animator.runtimeAnimatorController.animationClips)
        {

            if (a.name.Contains("Reload"))
            {
                AnimationEvent e = new AnimationEvent();
                e.time = a.length;
                e.functionName = "ReloadWeapon";
                a.AddEvent(e);
            }
            else if (a.name.Contains("Fire"))
            {
                AnimationEvent e = new AnimationEvent();
                e.time = 0;
                e.functionName = "FireWeapon";
                a.AddEvent(e);
            }
        }
        */
    }
    void EndFire()
    {
        this.transform.localPosition = startPos;
        isFiring = false;
    }
    void FireWeapon()
    {
        if (!isFiring) return;
        for(int i = 0; i < numOfBullets; i++) { 
            Vector3 camDir = Camera.main.transform.forward;
            if(weaponSpread > 0)
            {
                float ang = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float yaw = Random.Range(0, weaponSpread) * Mathf.Deg2Rad;
                Vector3 newDir = new Vector3(Mathf.Sin(yaw)* Mathf.Cos(ang)
                    , Mathf.Sin(yaw)* Mathf.Sin(ang), Mathf.Cos(yaw));
                newDir = Camera.main.transform.TransformDirection(newDir);
                camDir = newDir.normalized;
            }
            Vector3 dir = this.transform.GetChild(0).position - this.transform.position;
            RaycastHit info;
            bool hit = GamePhysics.AttackRayCast(new Ray(Camera.main.transform.position, camDir), dmg, range, out info); 
                //Physics.Raycast(this.transform.GetChild(0).position, dir,out info,range);
            if (hit && info.collider.tag == "Enemy") Debug.Log("HIT! THIS WOULD DAMAGE AN ENEMY");
            if (!hit)
            {
                info.point = Camera.main.transform.position + camDir.normalized * range;
                    //this.transform.GetChild(0).position +  dir.normalized * range;
            }
            TrailRenderer t = ObjectLoader.LoadObject(hitscanBullet.name).GetComponent<TrailRenderer>();
            t.transform.position = this.transform.GetChild(0).position;
                //Instantiate(hitscanBullet, this.transform.GetChild(0).position, Quaternion.identity);
            StartCoroutine(HitscanTrail(t,info));
        }
        barrel.PlayOneShot(fireSound);
        //PUT DAMAGE CODE BELOW
        //info.collider.GetComponent<Enemy>().takeDamage();
        
        ammoCount -= ammoPerBullet*numOfBullets;
        if (ammoCount < 0) ammoCount = 0;
        if (!UsesUI)
        {
            float rVal = ammoCount / ammoTotal;
            rVal = 2f * rVal - 1f;
            rVal = -rVal;
            rVal = Mathf.Clamp(rVal, -1f, 1f);
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetFloat("_AmmoLevel", rVal);
            renderer.SetPropertyBlock(block);
        }
        else
        {
            ammoUI.GetComponent<TMP_Text>().text = ammoCount.ToString();
        }
    }
    IEnumerator HitscanTrail(TrailRenderer t,RaycastHit info)
    {
        float time = 0;
        Vector3 start = t.transform.position;
        while(time < 1)
        {
            t.transform.position = Vector3.Lerp(start, info.point, time);
            time += Time.deltaTime/t.time;
            yield return null;
        }
        t.transform.position = info.point;
        t.transform.gameObject.SetActive(false);
        //Destroy(t.gameObject, t.time);
    }
    void FireWeaponBullet()
    {
        if (!isFiring) return;
        GameObject g = ObjectLoader.LoadObject(bullet.name);
            //Instantiate<GameObject>(bullet);
        g.transform.position = this.transform.GetChild(0).position;
        g.GetComponent<Bullet>().dir = this.transform.GetChild(0).position - this.transform.position;
        ammoCount -= ammoPerBullet;
        if (ammoCount < 0) ammoCount = 0;
        if (!UsesUI)
        {
            float rVal = ammoCount / ammoTotal;
            rVal = 2f * rVal - 1f;
            rVal = -rVal;
            rVal = Mathf.Clamp(rVal, -1f, 1f);
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetFloat("_AmmoLevel", rVal);
            renderer.SetPropertyBlock(block);
        }
        else
        {
            ammoUI.GetComponent<TMP_Text>().text = ammoCount.ToString();
        }

    }
    void ReloadWeapon()
    {
        if(isReloading){
            isReloading = false;
            ammoCount = ammoTotal;
            animator.SetBool("IsReloading", false);
            if (!UsesUI)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(block);
                block.SetFloat("_AmmoLevel", -1f);
                renderer.SetPropertyBlock(block);
            }
            else
            {
                ammoUI.GetComponent<TMP_Text>().text = ammoCount.ToString();
            }
            barrel.PlayOneShot(reloadSound);
            PlayerManager.instance.gunsReloading--;
            Debug.Log(this.transform.gameObject.name);
        }
        Debug.Log("TESTING!");
    }
    public void  ApplyGunData(GunData data)
    {
        firerate = data.firerate;
        dmg = data.dmg;
        reloadSpeed = data.reloadSpeed;
        ammoTotal = data.ammoTotal;
        range = data.range;
        ammoPerBullet = data.ammoPerBullet;
        if (this.UsesUI)
        {
            ammoUI.GetComponent<TMP_Text>().text = ammoCount.ToString();
        }
        else
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetColor("_LevelColor", data.gunColor);
            renderer.SetPropertyBlock(block);

        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!PauseMenu.GameIsPaused)
        {
            //Debug.Log(PlayerManager.instance.gunsReloading);
            //Debug.Log("GUn count: " + PlayerManager.instance.gunCount);
            if (Input.GetMouseButton(0) && !isReloading && PlayerManager.instance.gunsReloading < 1
                && (isAuto || !hasFired))
            {
                //Keep it nested so that reactions to running out of ammo can be handled
                if (ammoCount > 0)
                {
                    hasFired = true;
                    animator.SetBool("IsFiring", true);
                    isFiring = true;
                }
                else
                {
                    if (!barrel.isPlaying) barrel.PlayOneShot(clickSound);
                    animator.SetBool("IsFiring", false);
                    EndFire();
                }
                //FireWeapon();
            }
            else
            {
                if (Input.GetMouseButtonUp(0)) hasFired = false;
                animator.SetBool("IsFiring", false);
                EndFire();
            }
            if(Input.GetKeyDown(KeyCode.R) && !isReloading && ammoCount != ammoTotal)
            {
                isReloading = true;
                PlayerManager.instance.gunsReloading++;
                animator.SetBool("IsReloading", true);
            }
        }
    }
}
