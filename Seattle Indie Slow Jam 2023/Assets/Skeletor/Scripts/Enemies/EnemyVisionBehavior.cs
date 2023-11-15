using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// created by skeletor
// controls an aimconstraint used to rotate the upper body of the enemy to look at a target
// this component is attached the rig layer of an enemy behavior. This component is also optional for enemies that do not want to rotate their upper body
public class EnemyVisionBehavior : MonoBehaviour
{
    // reference to an empty transform the view constraint will twist the upper body to look at
    [SerializeField] private Transform viewTarget;
    // reference to the aim constraint that applies motion to the rig after animation
    [SerializeField] private MultiAimConstraint _spine0;
    // reference to the aim constraint that applies motion to the rig after animation
    [SerializeField] private MultiAimConstraint _spine1;
    // reference to the aim constraint that applies motion to the rig after animation
    [SerializeField] private MultiAimConstraint _spine2;
    // reference to the actual enemy behavior
    [SerializeField] private EnemyBehavior _myBehavior;
    // determines how much of the weight is distributed between the spine 0 and 1 when turning
    [SerializeField, Range(0,1)] private float _spine0Ratio;
    // determines how much of the weight is distributed between the spine 1 and 2 when turning
    [SerializeField, Range(0,1)] private float _spine1Ratio;
    // determines how much control the aim constraint has over the body. 0 = no control, the body will not turn 1 = full control, the body will snap to position
    private float _targetWeight;

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _spine0.weight = Mathf.Lerp(_spine0.weight, _targetWeight * _spine0Ratio, Time.deltaTime * 5);
        _spine1.weight = Mathf.Lerp(_spine1.weight, _targetWeight * _spine1Ratio, Time.deltaTime * 5);
        _spine2.weight = Mathf.Lerp(_spine2.weight, _targetWeight * (1-_spine1Ratio), Time.deltaTime * 5);
        if(_myBehavior.LockedOnTarget)
        {
            viewTarget.position = _myBehavior.LookTarget.position;
            _targetWeight = 1;
        }    
        else
        {
            _targetWeight = 0; 
        }
    }
}
