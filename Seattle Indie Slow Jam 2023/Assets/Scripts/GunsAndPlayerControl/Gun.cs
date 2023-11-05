using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float firerate = 0.5f;
    public float dmg = 1f;
    public float reloadSpeed = 1f;
    public float ammoTotal = 100f;
    public float ammoPerBullet = 1f;
    public AnimationClip fireAni;
    public AnimationClip reloadAni;
    public GameObject bullet;
    float ammoCount;
    Animator animator;
    bool isReloading = false;
    float firetimer = 0f;
    float reloadtimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        //animator.SetFloat("fireSpeed", firerate);
        //animator.SetFloat("reloadSpeed", reloadSpeed);
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
        GameObject g = Instantiate<GameObject>(bullet);
        g.transform.position = this.transform.GetChild(0).position;
        g.GetComponent<Bullet>().dir = this.transform.GetChild(0).position - this.transform.position;
        ammoCount -= ammoPerBullet;
        if (ammoCount < 0) ammoCount = 0;
    }
    void ReloadWeapon()
    {
        reloadtimer = 0f;
        ammoCount = ammoTotal;
        animator.SetBool("IsReloading", false);
        isReloading = false;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(animator.GetCurrentAnimatorStateInfo(0));
        if (Input.GetMouseButton(0) && !isReloading)
        {
            animator.SetBool("IsFiring", true);
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
