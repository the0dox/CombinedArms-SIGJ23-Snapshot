using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// simple component that returns true if the attached object is touching the ground this frame
public class GroundedCheck : MonoBehaviour
{
    // public accessor
    public bool Grounded => _grounded;
    // returns true if this gameobject is touching the ground this frame
    private bool _grounded;
    // declares what the ground check considers the ground
    [SerializeField] private LayerMask groundMask;
    // the height of this gameobject
    private float yBound;
    // an additional length added to the ground check to account for physics inaccuracy
    private float skinDepth = 0.15f;

    // Start is called before the first frame update
    void Awake()
    {
        yBound = GetComponent<Collider>().bounds.size.y/2;
    }

    // Update is called once per frame
    void Update()
    {
        _grounded = Physics.Raycast(transform.position + new Vector3(0,yBound,0), Vector3.down, yBound + skinDepth, groundMask);
        Debug.DrawRay(transform.position + new Vector3(0,yBound,0), Vector3.down, _grounded ? Color.green : Color.red);
    }
}
