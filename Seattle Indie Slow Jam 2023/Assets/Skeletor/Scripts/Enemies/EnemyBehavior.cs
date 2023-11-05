using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// created by Skeletor
// primary state machine that drives enemy behavior
public class EnemyBehavior : StateController<EnemyBehavior>
{
    // public accessors
    public Animator AnimationComponent => _animationComponent;
    public NavMeshAgent MyAgent => _myAgent;
    public Rigidbody MyBody => _myBody;
    
    // used to control movement
    [SerializeField] private NavMeshAgent _myAgent;
    // used to control animation
    [SerializeField] private Animator _animationComponent;
    // controls physics for this object
    [SerializeField] private Rigidbody _myBody;
    [SerializeField] private float _upperBodyRotation;

    // current velocity in local space this frame
    private Vector3 _relativeVelocity;
    private Transform _lookTransform;
    
    // enemy states
    public readonly EnemyPatrolBehavior patrol = new EnemyPatrolBehavior();
    public readonly EnemyIdleState idle = new EnemyIdleState();

    // on first frame assume patrol behavior
    void Awake()
    {
        SetState(idle);
    }

    // each frame, update animations and states
    protected override void Update()
    {
        AnimateMotion();
        base.Update();
    }

    public void SetLookTarget(Transform newTransform = null)
    {
        _lookTransform = newTransform;
    }

    // sets animation values based on current velocity
    void AnimateMotion()
    {
        _relativeVelocity = transform.InverseTransformVector(_myAgent.velocity);
        //Debug.LogFormat($"current relative velocity {_relativeVelocity}");
        _animationComponent.SetFloat("Vertical_f", _relativeVelocity.z);
        _animationComponent.SetFloat("Horizontal_f", _relativeVelocity.x);

        if(_lookTransform == null)
        {
            _animationComponent.SetFloat("Upper_Horizontal_f", 0);
            _animationComponent.SetFloat("Upper_Vertical_f", 0);
            return;
        }

        Vector3 lookAngle = (_lookTransform.position - transform.position).normalized;
        float relativeHorizontalAngle = Vector3.SignedAngle(transform.forward, new Vector3(lookAngle.x, transform.forward.y, lookAngle.z), Vector3.up);
        float relativeVerticalAngle = Vector3.SignedAngle(transform.forward, lookAngle, -transform.right);
        //Debug.LogFormat($"Relative Y Angle {relativeVerticalAngle}")
        if(Mathf.Abs(relativeHorizontalAngle) > _upperBodyRotation)
            relativeHorizontalAngle = 0;
        if(Mathf.Abs(relativeVerticalAngle) > _upperBodyRotation)
            relativeVerticalAngle = 0;
        _animationComponent.SetFloat("Upper_Horizontal_f", relativeHorizontalAngle, -_upperBodyRotation, _upperBodyRotation);
        _animationComponent.SetFloat("Upper_Vertical_f", relativeVerticalAngle, -_upperBodyRotation, _upperBodyRotation);
    }
}
