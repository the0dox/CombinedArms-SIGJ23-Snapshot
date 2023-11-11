using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New GunData",menuName ="Assets/GunData")]
public class GunData : ScriptableObject
{
    [SerializeField]
    public float firerate = 0.5f;
    [SerializeField]
    Vector2 firerateRange = new Vector2(.5f, 3.5f);
    [SerializeField]
    public float dmg = 1f;
    [SerializeField]
    Vector2 dmgRange = new Vector2(2f, 3f);
    [SerializeField]
    public float reloadSpeed = 1f;
    [SerializeField]
    Vector2 reloadSpeedRange = new Vector2(.5f,3.5f);
    [SerializeField]
    public float spreadAngle = 0f;
    [SerializeField]
    Vector2 spreadAngleRange = new Vector2(0f, 45f);
    [SerializeField]
    public int numOfBullets = 1;
    [SerializeField]
    Vector2 numOfBulletsRange = new Vector2(1, 6);
    [SerializeField]
    public float ammoTotal = 100f;
    [SerializeField]
    public float ammoPerBullet = 1f;
    [SerializeField]
    public float range = 10f;
    [SerializeField]
    public Color gunColor = Color.red;
    [SerializeField]
    public GameObject gunPrefab;

    //Enemy checks
    [SerializeField]
    public bool isEnemy = false;
    [SerializeField]
    public GunData gunDrop;

    public void RandomizeProperties()
    {
        firerate = Random.Range(firerateRange.x, firerateRange.y);
        dmg = Random.Range(dmgRange.x, dmgRange.y);
        reloadSpeed = Random.Range(reloadSpeedRange.x, reloadSpeedRange.y);
        gunColor = new Color(Random.Range(.3f, 1f),
            Random.Range(.3f, 1f), Random.Range(.3f, 1f), 1f);
        spreadAngle = Random.Range(spreadAngleRange.x, spreadAngleRange.y);
        numOfBullets = Random.Range((int)numOfBulletsRange.x, (int)numOfBulletsRange.y);

    }
}
