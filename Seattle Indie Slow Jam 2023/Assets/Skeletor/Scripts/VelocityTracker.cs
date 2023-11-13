using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by skeletor
// attach this to a gameobject to get its current velocity at any given time
public class VelocityTracker : MonoBehaviour
{
    // public accessor
    public Vector3 CurrentVelocity => _velocity;

    // the velocity of this transform this frame. Note that this is world velocity not relative
    private Vector3 _velocity;
    // reference to the previous positions this object was in before this one
    private Vector3[] _previousPositions = new Vector3[10];

    // Start is called before the first frame update
    void OnEnable()
    {
        for(int i = 0; i < _previousPositions.Length; i++)
        {
            _previousPositions[i] = transform.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    void LateUpdate()
    {
        CalculateVelocity();
    }

    // updates velocity value by comparing cached positions
    void CalculateVelocity()
    {
        _velocity = Vector3.zero;
        for(int i = _previousPositions.Length -1 ; i > 0; i--)
        {
            _velocity += _previousPositions[i - 1] - _previousPositions[i];
            _previousPositions[i] = _previousPositions[i - 1]; 
        }
        _previousPositions[0] = transform.position;
        _velocity /= _previousPositions.Length - 1 ;
        _velocity /= Time.deltaTime;
        Debug.DrawRay(transform.position, _velocity, Color.blue);
    }
}
