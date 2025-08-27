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

    [Header("Audio")]
    public AudioClip hitSound;
    protected AudioSource audioSource;

    protected State currentState;
    protected NavMeshAgent agent;
    protected Transform playerTarget;
    protected Animator animator;
    protected bool isDead = false;

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

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Chasing:
                HandleChasingState();
                break;
            case State.Attacking:
                HandleAttackingState();
                break;
            case State.Death:
                HandleDeathState();
                break;
        }

        Debug.Log(currentState);
    }

    protected abstract void HandleIdleState();
    protected abstract void HandleChasingState();
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
}