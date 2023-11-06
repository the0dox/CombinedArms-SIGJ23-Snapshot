using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
# endif 

// created by Skeletor
// primary state machine that drives enemy behavior
public class EnemyBehavior : StateController<EnemyBehavior>, IAttackable
{
    // public accessors
    public Animator AnimationComponent => _animationComponent;
    public NavMeshAgent MyAgent => _myAgent;
    public Rigidbody MyBody => _myBody;
    public Transform LookTarget => _lookTarget;
    public float AttackRadius => _attackRadius;
    public float AttackCoolDownDuration => _attackCooldownDuration;
    public Transform VisionTransform => _visionTransform;
    public bool AttackCoolDown {get => _attackCooldown; set => _attackCooldown = value;}
    public State<EnemyBehavior> Idle => _idle;
    public State<EnemyBehavior> Patrol => _patrol;
    public State<EnemyBehavior> Approach => _approach;
    public State<EnemyBehavior> Attack => _attack;
    
    
    // used to control movement
    [SerializeField] private NavMeshAgent _myAgent;
    // used to control animation
    [SerializeField] private Animator _animationComponent;
    // controls physics for this object
    [SerializeField] private Rigidbody _myBody;
    // the max rotation at which the upper body will snap to the front
    [SerializeField, Range(0.0f, 120.0f)] private float _upperBodyRotation;
    // the radius at which the enemy will detect the player
    [SerializeField, Range(0.0f, 50.0f)] private float _visionRadius;
    // the radius at which the enemy will attack the player
    [SerializeField, Range(0.0f, 50.0f)] private float _attackRadius;
    // the transform that the enemy draws vision and attacks from
    [SerializeField] private Transform _visionTransform;
    // the time in seconds the enemy must wait before attacking again
    [SerializeField] private float _attackCooldownDuration;
    // speed that the enemy can rotate its transform
    [SerializeField] private float _turnSpeed;
    // the amount of health this enemy starts with
    [SerializeField] private float _startingHealth;
    // reference to the ragdoll attached to my model
    [SerializeField] private RagdollBehavior _myRagdoll; 
    [SerializeField] private Collider _myCollider;
    // the current amount of health this enemy has
    private float _health;
    // returns true if this enemy can attack this frame
    private bool _attackCooldown;
 
    // current velocity in local space this frame
    private Vector3 _relativeVelocity;
    // the current transform the enemy is trying to look at
    private Transform _lookTarget;
    
    // enemy states should be overriden for different enemy variants
    // starts in idle state by default
    protected State<EnemyBehavior> _idle;
    // alternates to patrol until player approaches
    protected State<EnemyBehavior> _patrol;
    // approaches player within range
    protected State<EnemyBehavior> _approach;
    // attacks if within attack range
    protected State<EnemyBehavior> _attack;

    // assign states before first frame
    protected virtual void Awake()
    {
        OnSpawn();
        _animationComponent.enabled = true;
        MyBody.detectCollisions = true;
        MyBody.useGravity = false;
        _myCollider.enabled = true;
        _health = _startingHealth;
        SetState(_idle);
    }

    // on first frame assume patrol behavior
    protected virtual void OnSpawn()
    {
        _patrol = new EnemyPatrolState();
        _idle = new EnemyIdleState();
        _approach = new EnemyRepositionState();
        _attack = new EnemyAttackState();
    }

    // each frame, update animations and states
    protected override void Update()
    {
        AnimateMotion();
        base.Update();
    }

    // called when a projectile comes into contact with an enemy
    public void TakeDamage(GameObject source, float damage)
    {
        _health -= damage;
        OnHurt();
        if(_health < 0)
            Die(source);
    }   
    
    protected virtual void OnHurt()
    {
        _animationComponent.SetTrigger("Hurt");
    }

    // called when the enemy has been reduced to zero hit points
    protected virtual void Die(GameObject source = null)
    {
        _myAgent.enabled = false;
        _animationComponent.enabled = false;
        MyBody.detectCollisions = false;
        MyBody.useGravity = true;
        _myCollider.enabled = false;
        this.enabled = false;
        _myRagdoll.EnableRagdoll(true);
        Invoke("Unload", 10f);
        if(source != null)
            _myRagdoll.ApplyRagdollForce(source.transform.position, 35);
        //gameObject.SetActive(false);
        //transform.position = Vector3.zero;
    }

    void Unload()
    {
        gameObject.SetActive(false);
        this.enabled = true;
    }

    // sets a new transform as the look target of this enemy
    public void SetLookTarget(Transform newTransform = null)
    {
        _lookTarget = newTransform;
    }

    // checks to see if the player is within the vision range, switches the enemy to approach the enemy
    public void LookForPlayer()
    {
        if(Vector3.Distance(_visionTransform.position, DebugTestCamera.PlayerObject.transform.position) < _visionRadius)
        {
            SetLookTarget(DebugTestCamera.PlayerObject.transform);
            SetState(_approach);
        }
    }

    public void RotateTowardsTarget()
    {
        if(_lookTarget == null)
        {
            return;
        } 
        Vector3 targetDirection = (_lookTarget.position - transform.position).normalized;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection) , Time.deltaTime * _turnSpeed);
    }

    // sets animation values every frame
    void AnimateMotion()
    {
        _relativeVelocity = transform.InverseTransformVector(_myAgent.velocity);
        //Debug.LogFormat($"current relative velocity {_relativeVelocity}");
        _animationComponent.SetFloat("Vertical_f", _relativeVelocity.z);
        _animationComponent.SetFloat("Horizontal_f", _relativeVelocity.x);

        // sets upper body rotation directly ahead if there is no look target
        if(_lookTarget == null)
        {
            _animationComponent.SetFloat("Upper_Horizontal_f", 0, 0.5f, Time.deltaTime);
            _animationComponent.SetFloat("Upper_Vertical_f", 0, 0.5f, Time.deltaTime);
            return;
        }

        // if there is a look target get the relative x and y rotation to that target
        Vector3 lookAngle = (_lookTarget.position - _visionTransform.position).normalized;
        float relativeHorizontalAngle = Vector3.SignedAngle(_visionTransform.forward, new Vector3(lookAngle.x, _visionTransform.forward.y, lookAngle.z), Vector3.up);
        float relativeVerticalAngle =  Vector3.SignedAngle(_visionTransform.forward, lookAngle, -_visionTransform.right);
        //Debug.LogFormat($"Relative Y Angle {relativeVerticalAngle}")
        if(Mathf.Abs(relativeHorizontalAngle) > _upperBodyRotation)
            relativeHorizontalAngle = 0;
        if(Mathf.Abs(relativeVerticalAngle) > _upperBodyRotation)
            relativeVerticalAngle = 0;
        // adjusts upper body rotation
        _animationComponent.SetFloat("Upper_Horizontal_f", relativeHorizontalAngle, 0.3f, Time.deltaTime);
        _animationComponent.SetFloat("Upper_Vertical_f", relativeVerticalAngle, 0.3f, Time.deltaTime);
    }

    #if UNITY_EDITOR
    // displays information of the enemy in the editor
    void OnDrawGizmos()
    {   
        Gizmos.DrawWireSphere(_visionTransform.position, _lookTarget == null ? _visionRadius : _attackRadius);
        Handles.Label(_visionTransform.position, ToString());
        if(_myAgent.destination != null)
            Gizmos.DrawSphere(_myAgent.destination, 0.25f);
        if(_lookTarget != null)
            Gizmos.DrawLine(_visionTransform.position, _lookTarget.position);
    }
    #endif

    // simple debug string that displays the current state of the enemy
    public override string ToString()
    {
        string targetString = _lookTarget != null ? _lookTarget.name : "";
        return string.Format($"{gameObject.name}\nHealth:{_health}/{_startingHealth}\nState: {_currentState}\nLook Target: {targetString}");
    }
}
