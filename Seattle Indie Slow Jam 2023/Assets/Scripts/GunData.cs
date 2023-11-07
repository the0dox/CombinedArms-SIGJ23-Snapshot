using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunData : ScriptableObject
{
    [SerializeField]
    public float firerate = 0.5f;
    [SerializeField]
    Vector2 firerateRange = new Vector2(.5f, 3.5f);
    [SerializeField]
    public float dmg = 1f;
    [SerializeField]
    Vector2 dmgRange = new Vector2(.5f, 1.5f);
    [SerializeField]
    public float reloadSpeed = 1f;
    [SerializeField]
    Vector2 reloadSpeedRange = new Vector2(.5f,3.5f);
    [SerializeField]
    public float ammoTotal = 100f;
    [SerializeField]
    public float ammoPerBullet = 1f;
    [SerializeField]
    public float range = 10f;
    [SerializeField]
    public Color gunColor = Color.red;

    public void RandomizeProperties()
    {
        firerate = Random.Range(firerateRange.x, firerateRange.y);
        dmg = Random.Range(dmgRange.x, dmgRange.y);
        reloadSpeed = Random.Range(reloadSpeedRange.x, reloadSpeedRange.y);
        gunColor = new Color(Random.Range(.3f, 1f),
            Random.Range(.3f, 1f), Random.Range(.3f, 1f), 1f);
    }
}
