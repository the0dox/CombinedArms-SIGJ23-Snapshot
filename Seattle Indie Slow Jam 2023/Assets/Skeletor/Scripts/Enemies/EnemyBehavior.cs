using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GameFilters;
using Unity.Mathematics;
using UnityEngine.Animations.Rigging;



#if UNITY_EDITOR
using UnityEditor;
# endif 

// created by Skeletor
// primary state machine that drives enemy behavior
[RequireComponent(typeof(VelocityTracker)), DefaultExecutionOrder(100)]
public class EnemyBehavior : StateController<EnemyBehavior>, IAttackable
{
    // public accessors
    public Animator AnimationComponent => _animationComponent;
    public NavMeshAgent MyAgent => _myAgent;
    public Rigidbody MyBody => _myBody;
    public Transform LookTarget => _lookTarget;
    public float AttackRadius => _attackRadius;
    public float RepositionVariance => _repositionVariance;
    public float RepositionRefreshRate => _repositionRefreshRate;
    public Transform VisionTransform => _visionTransform;
    public bool AttackCoolDown => _attackCooldown;
    public bool Grounded => _myGroundCheck.Grounded;
    public State<EnemyBehavior> Idle => _idle;
    public State<EnemyBehavior> Patrol => _patrol;
    public State<EnemyBehavior> Approach => _approach;
    public State<EnemyBehavior> Attack => _attack;
    public EnemyFallingState Falling => _falling;
    public EnemyLoadoutBehavior Loadout => _loadout;
    public bool LockedOnTarget => _lockedOnTarget;
    public AudioClip HurtSound => _hurtSound;
    public AudioClip DeathSound => _deathSound;
    public Vector3 OriginalPosition => _originalPosition;
    public EventHandler Injured;
    public EventHandler SeenPlayer;
    public EventHandler Killed;



    // the amount of health this enemy starts with
    [SerializeField] private float _startingHealth;
    // the max rotation at which the upper body will snap to the front
    [SerializeField, Range(0.0f, 120.0f)] private float _upperBodyRotation;
    // the radius at which the enemy will detect the player
    [SerializeField, Range(0.0f, 50.0f)] private float _visionRadius;
    // the radius at which the enemy will attack the player
    [SerializeField, Range(0.0f, 50.0f)] private float _attackRadius;
    // the radius at which the enemy will attack the player
    [SerializeField, Range(0.0f, 1f)] private float _dropRate;
    // how much will the enemy aim to move within it's minimum attack range
    [SerializeField] private float _repositionVariance;
    // how often should the enemy choose to recalculate their move destination
    [SerializeField] private float _repositionRefreshRate;
    // speed that the enemy can rotate its transform
    [SerializeField] private float _turnSpeed;
    // reference to any possible weapon prefabs this enemy could equip
    [SerializeField] private GameObject _startingWeapon;
    // the transform that the enemy draws vision and attacks from
    [SerializeField] protected Transform _visionTransform;
    // the time in seconds the enemy must wait before attacking again
    [SerializeField] private FloatRange _attackCooldownDuration;
    // used to control movement
    [SerializeField] private NavMeshAgent _myAgent;
    // used to control animation
    [SerializeField] private Animator _animationComponent;
    // controls physics for this object
    [SerializeField] private Rigidbody _myBody;
    // used to spawn a rag doll on death
    [SerializeField] private RagdollSpawner myRagdoll;
    // used to track when animations have been completed
    [SerializeField] private AnimationEventHandler _animationHandler;
    // used to check every frame if the enemy is on the ground
    [SerializeField] private GroundedCheck _myGroundCheck;
    // used to handle any weapons this enemy has equipped
    [SerializeField] private EnemyLoadoutBehavior _loadout;
    private VelocityTracker _myVelocity;
    // the current amount of health this enemy has
    private float _health;
    // returns true if this enemy can attack this frame
    private bool _attackCooldown;
    // returns true if the enemy's upper body rotation is close enough to looking at its target this frame
    private bool _lockedOnTarget;
    private AudioSource _myAudio;
    [SerializeField] private AudioClip _hurtSound;
    [SerializeField] private AudioClip _deathSound;
 
    // current velocity in local space this frame
    private Vector3 _relativeVelocity;
    // the current transform the enemy is trying to look at
    private Transform _lookTarget;
    // original world position the enemy started in
    private Vector3 _originalPosition;
    // randomizes enemy sound pitches
    private readonly FloatRange _enemyPitch = new FloatRange(0.9f, 1);
    
    // enemy states should be overriden for different enemy variants
    // starts in idle state by default
    protected State<EnemyBehavior> _idle;
    // alternates to patrol until player approaches
    protected State<EnemyBehavior> _patrol;
    // approaches player within range
    protected State<EnemyBehavior> _approach;
    // attacks if within attack range
    protected State<EnemyBehavior> _attack;
    // stunned for a brief moment after damage
    protected State<EnemyBehavior> _hurt;
    // state when not touching the ground 
    protected EnemyFallingState _falling = new EnemyFallingState();

    // assign states before first frame
    void OnEnable()
    {
        OnSpawn();
        _animationComponent.enabled = true;
        _health = _startingHealth;
        _attackCooldown = false;
        _myBody.velocity = Vector3.zero;
        _originalPosition = transform.position;
        _myAudio = GetComponentInChildren<AudioSource>();
        _myVelocity = GetComponent<VelocityTracker>();
        SetState(_falling);
    }

    // on first frame assume patrol behavior
    protected virtual void OnSpawn()
    {
        _patrol = new EnemyPatrolState();
        _idle = new EnemyIdleState();
        _approach = new EnemyRepositionState();
        _hurt = new EnemyInjuredState();
        Loadout.EquipWeapon(_startingWeapon);
        _attack = Loadout.ActiveWeapon.AttackState;
        TriggerAttackCoolDown();
    }

    // each frame, update animations and states
    protected override void Update()
    {
        AnimateMotion();
        base.Update();
    }

    // called when a projectile comes into contact with an enemy
    public void TakeDamage(Vector3 source, float damage)
    {
        if(gameObject.activeInHierarchy)
        {
            _health -= damage;
            GameObject bloodParticle = ObjectLoader.LoadObject("BloodParticleFX", true);
            Vector3 enemyToSouce = transform.position - source;
            bloodParticle.transform.position = _visionTransform.position;
            bloodParticle.transform.rotation = Quaternion.LookRotation(enemyToSouce);
            Injured?.Invoke(this, EventArgs.Empty);
            SetState(_hurt);
            if(_health <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }   

    // need to wait one frame to die to allow velocity to properly transfer to ragdoll
    IEnumerator Die()
    {
        // wait for two frames
        for(int i = 0; i < 2; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        try
        {
            ToggleNavAgent(false); 
            myRagdoll.SpawnRagdoll();  
            Killed?.Invoke(this, EventArgs.Empty);   
            OnDeath();
        }
        finally
        {
            gameObject.SetActive(false);
            transform.position = Vector3.zero;
        }
    }

    void OnDisable()
    {
        Injured = null;
        SeenPlayer = null;
        Killed = null;
    }

    // called when the enemy has been reduced to zero hit points
    protected virtual void OnDeath()
    {
        AudioSource.PlayClipAtPoint(_deathSound, _visionTransform.position, 3);
        _loadout.DropWeapon(_dropRate);       
    }

    // can be used to enable/disable the status of alpha* movement of the enemy
    public void ToggleNavAgent(bool enabled)
    {
        _myAgent.enabled = enabled;
        _myBody.useGravity = !enabled;
        if(enabled)
        {
            _myBody.velocity = Vector3.zero;
        }
    }

    // sets a new transform as the look target of this enemy
    public void SetLookTarget(Transform newTransform = null)
    {
        _lookTarget = newTransform;
    }

    // checks to see if the player is within the vision range, switches the enemy to approach the enemy
    public void LookForPlayer()
    {
        if(_lookTarget != null)
        {
            SetState(_approach);    
        } 
        else if(PlayerManager.instance != null)
        {
            Vector3 enemyToPlayer = PlayerManager.instance.transform.position - _visionTransform.position;
            if(enemyToPlayer.magnitude < _visionRadius && !Physics.Raycast(_visionTransform.position, enemyToPlayer, enemyToPlayer.magnitude, LayerMask.GetMask("Terrain", "QueryAttack")))
            {
                SeenPlayer?.Invoke(this, EventArgs.Empty);
                DetectPlayer();
            }
        }
    }

    public void DetectPlayer()
    {
        SetLookTarget(PlayerManager.instance.transform);
        SetState(_approach);
    }

    // rotates the transform (not the upper body) of the enemy towards its look target
    public bool RotateTowardsTarget()
    {
        if(_lookTarget == null)
        {
            return false;
        } 
        Vector3 targetDirection = (_lookTarget.position - transform.position).normalized;
        Quaternion lookTarget = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookTarget, Time.deltaTime * _turnSpeed);
        return Mathf.Abs(transform.rotation.eulerAngles.y - lookTarget.eulerAngles.y) < 0.025f; 
    }

    public void ApplyRootMotion()
    {
        transform.position += _animationHandler.RootPositionDelta;
        transform.rotation *= _animationHandler.RootRotationDelta;
    }

    // sets animation values every frame
    void AnimateMotion()
    {
        _relativeVelocity = transform.InverseTransformVector(_myVelocity.CurrentVelocity);
        _animationComponent.SetFloat("Vertical_f", _relativeVelocity.z, 0.1f,  Time.unscaledDeltaTime);
        _animationComponent.SetFloat("Horizontal_f", _relativeVelocity.x, 0.1f, Time.unscaledDeltaTime);
        if(float.IsNaN(_animationComponent.GetFloat("Vertical_f")))
        {
            _animationComponent.SetFloat("Vertical_f", 0);
        }   
        if(float.IsNaN(_animationComponent.GetFloat("Horizontal_f")))
        {
            _animationComponent.SetFloat("Horizontal_f", 0);
        }
    
        if(_lookTarget == null)
        {
            _lockedOnTarget = false;   
            return; 
        }

        // sets upper body rotation directly ahead if there is no look target
        if(_lookTarget == null)
        {
            
            _lockedOnTarget = false;
            return;
        }

        // if there is a look target get the relative x and y rotation to that target
        Vector3 lookAngle = (_lookTarget.position - _visionTransform.position).normalized;
        float relativeHorizontalAngle = Vector3.SignedAngle(_visionTransform.forward, new Vector3(lookAngle.x, _visionTransform.forward.y, lookAngle.z), Vector3.up);
        float relativeVerticalAngle =  Vector3.SignedAngle(_visionTransform.forward, lookAngle, -_visionTransform.right);
        //Debug.LogFormat($"Relative Y Angle {relativeVerticalAngle}")
        _lockedOnTarget = true;
        if(Mathf.Abs(relativeHorizontalAngle) > _upperBodyRotation)
        {  
            _lockedOnTarget = false;
        }
        else if(Mathf.Abs(relativeVerticalAngle) > _upperBodyRotation)
        { 
            _lockedOnTarget = false;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        _myAudio.pitch = _enemyPitch.RandomValue;
        _myAudio.PlayOneShot(clip);
    }

    public void TriggerAttackCoolDown()
    {
        _attackCooldown = true;
        this.Invoke(() => _attackCooldown = false, _attackCooldownDuration.RandomValue);
    }

    public void ArenaModeSetup()
    {
        _dropRate = 1f;
    }


    #if UNITY_EDITOR
    // displays information of the enemy in the editor
    void OnDrawGizmos()
    {   
        //Gizmos.DrawWireSphere(_visionTransform.position, _lookTarget == null ? _visionRadius : _attackRadius);
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
        return string.Format($"{gameObject.name}\nHealth:{_health}/{_startingHealth}\nState: {_currentState}\nLook Target: {targetString} Locked On: {_lockedOnTarget}\nCan Attack: {_attackCooldown}");
    }
}
