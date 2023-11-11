using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// created by skeletor
// controls an aimconstraint used to rotate the upper body of the enemy to look at a target
// this component is attached the rig layer of an enemy behavior. This component is also optional for enemies that do not want to rotate their upper body
[RequireComponent(typeof(MultiAimConstraint))]
public class EnemyVisionBehavior : MonoBehaviour
{
    // reference to an empty transform the view constraint will twist the upper body to look at
    [SerializeField] private Transform viewTarget;
    // reference to the aim constraint that applies motion to the rig after animation
    private MultiAimConstraint _aimComponent;
    // reference to the actual enemy behavior
    [SerializeField] private EnemyBehavior _myBehavior;
    // determines how much control the aim constraint has over the body. 0 = no control, the body will not turn 1 = full control, the body will snap to position
    private float _targetWeight;

    // Start is called before the first frame update
    void Awake()
    {
        _aimComponent = GetComponent<MultiAimConstraint>();
    }

    // Update is called once per frame
    void Update()
    {
        _aimComponent.weight = Mathf.Lerp(_aimComponent.weight, _targetWeight, Time.deltaTime * 5);
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
