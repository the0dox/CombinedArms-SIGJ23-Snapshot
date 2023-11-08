using System.Collections;
using System.Collections.Generic;
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
    public AnimationClip fireAni;
    public AnimationClip reloadAni;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip clickSound;
    public GameObject bullet;
    public TrailRenderer hitscanBullet;
    AudioSource barrel;
    float ammoCount;
    Animator animator;
    MeshRenderer renderer;
    bool isReloading = false;

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
    }
    void FireWeapon()
    {
        Vector3 dir = this.transform.GetChild(0).position - this.transform.position;
        RaycastHit info;
        bool hit = GamePhysics.AttackRayCast(new Ray(this.transform.GetChild(0).position, dir), dmg, range, out info);
        if (!hit)
        {
            info.point = this.transform.GetChild(0).position +  dir.normalized * range;
        }
        TrailRenderer t = Instantiate(hitscanBullet, this.transform.GetChild(0).position, Quaternion.identity);
        StartCoroutine(HitscanTrail(t,info));
        barrel.PlayOneShot(fireSound);
        //PUT DAMAGE CODE BELOW
        //info.collider.GetComponent<Enemy>().takeDamage();
        
        ammoCount -= ammoPerBullet;
        if (ammoCount < 0) ammoCount = 0;
        float rVal = ammoCount / ammoTotal;
        rVal = 2f * rVal - 1f;
        rVal = -rVal;
        rVal = Mathf.Clamp(rVal, -1f, 1f);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetFloat("_AmmoLevel", rVal);
        renderer.SetPropertyBlock(block);

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
        Destroy(t.gameObject, t.time);
    }
    void FireWeaponBullet()
    {
        GameObject g = Instantiate<GameObject>(bullet);
        g.transform.position = this.transform.GetChild(0).position;
        g.GetComponent<Bullet>().dir = this.transform.GetChild(0).position - this.transform.position;
        ammoCount -= ammoPerBullet;
        if (ammoCount < 0) ammoCount = 0;
        float rVal = ammoCount / ammoTotal;
        rVal = 2f * rVal - 1f;
        rVal = -rVal;
        rVal = Mathf.Clamp(rVal, -1f, 1f);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetFloat("_AmmoLevel", rVal);
        renderer.SetPropertyBlock(block);

    }
    void ReloadWeapon()
    {
        ammoCount = ammoTotal;
        animator.SetBool("IsReloading", false);
        isReloading = false;
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetFloat("_AmmoLevel", -1f);
        renderer.SetPropertyBlock(block);
        barrel.PlayOneShot(reloadSound);
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(animator.GetCurrentAnimatorStateInfo(0));
        if (Input.GetMouseButton(0) && !isReloading)
        {
            //Keep it nested so that reactions to running out of ammo can be handled
            if (ammoCount > 0) animator.SetBool("IsFiring", true);
            else
            {
                if(!barrel.isPlaying) barrel.PlayOneShot(clickSound);
                animator.SetBool("IsFiring", false);
            }
            //FireWeapon();
        }
        else
        {
            animator.SetBool("IsFiring", false);
        }
        if(Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            isReloading = true;
            animator.SetBool("IsReloading", true);
        }
    }
}
