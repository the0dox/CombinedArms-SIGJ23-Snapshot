using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    [SerializeField]
    GameObject gunPrefab;
    public float health = 100f;
    public int gunCount = 0;
    Vector4 gunPlacementRange = new Vector4(-1, 1, -.8f, .8f);
    Vector2 placeOffsetAmount = Vector2.zero;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        placeOffsetAmount.x = gunPlacementRange.z - gunPlacementRange.x;
        placeOffsetAmount.y = gunPlacementRange.y - gunPlacementRange.w;
    }
    void PickupGun()
    {
        return;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
