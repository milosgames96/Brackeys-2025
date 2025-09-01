using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public abstract class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chasing,
        Attacking,
        Death
    }

    [Header("Base Stats")]
    [SerializeField] protected float health;
    [SerializeField] protected int damage;

    [Header("DEV Tweaks")]
    public float aggroRange = 40f;
    public float attackRange = 30f;

    [Header("Detection")]
    public float senseRadius = 12f;
    public float visionAngle = 90f;
    public LayerMask visionMask;
    public Transform eyeLocation;

    [Header("Audio")]
    public AudioClip hitSound;
    protected AudioSource audioSource;

    protected State currentState;
    protected NavMeshAgent agent;
    protected Transform playerTarget;
    protected Animator animator;
    protected bool isDead = false;

    public bool IsAlive()
    {
        return !isDead;
    }

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
        currentState = State.Idle;
    }

    protected virtual void Update()
    {
        if (playerTarget == null) return;

        bool canSeePlayer = CanSeePlayer();

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState(canSeePlayer);
                break;
            case State.Chasing:
                HandleChasingState(canSeePlayer);
                break;
            case State.Attacking:
                HandleAttackingState();
                break;
            case State.Death:
                HandleDeathState();
                break;
        }

        //Debug.Log(currentState);
    }

    protected abstract void HandleIdleState(bool canSeePlayer);
    protected abstract void HandleChasingState(bool canSeePlayer);
    protected abstract void HandleAttackingState();
    protected abstract void HandleDeathState();

    public abstract void Attack();

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        health -= amount;
        if (health <= 0)
        {
            Die();
            HandleDeathState();
        }
    }

    protected virtual void Die()
    {
        if (GameManager.instance != null && !isDead)
        {
            GameManager.instance.AddKill();
        }
        isDead = true;
        Destroy(gameObject, 10f);
    }

    /// <summary>
    /// This an ugly workaround to play 2D sounds since Unity doesn't have a built-in method for it.
    /// Will use it in any situation where ClipPoint is far away from the player.
    /// </summary>
    public void Play2DSound(AudioClip clip)
    {
        GameObject soundObject = new GameObject("Temp2DSound");

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        // Disable 3D
        audioSource.spatialBlend = 0.0f;

        audioSource.PlayOneShot(clip);
        Destroy(soundObject, clip.length);
    }

    private bool CanSeePlayer()
    {
        if (playerTarget == null) return false;

        Vector3 directionToPlayer = playerTarget.position - eyeLocation.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // This is "absolute" aggro range. So we prevent sneaking up behind
        // Works kinda like World of Warcraft's aggro system
        if (distanceToPlayer <= senseRadius)
        {
            return true;
        }

        // Check if the player is outside our aggro range
        if (distanceToPlayer > aggroRange)
        {
            return false;
        }

        // Angle check
        float angleToPlayer = Vector3.Angle(eyeLocation.forward, directionToPlayer);

        // If the player is outside the vision cone, we can't see them
        if (angleToPlayer > visionAngle / 2f)
        {
            return false;
        }

        // Obstruction check using raycast
        if (Physics.Raycast(eyeLocation.position, directionToPlayer.normalized, out RaycastHit hit, distanceToPlayer, visionMask))
        {
            // If the raycast hits the player, then we have a clear line of sight
            if (hit.transform == playerTarget)
            {
                return true;
            }
        }

        // If the raycast hit a wall or something else, de-aggro
        return false;
    }
}