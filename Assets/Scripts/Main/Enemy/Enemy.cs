using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle,
        Chasing,
        Attacking
    }

    [Header("Base Stats")]
    [SerializeField] protected float health;
    [SerializeField] protected int damage;

    [Header("DEV Tweaks")]
    public float aggroRange = 40f;
    public float attackRange = 30f;

    protected State currentState;
    protected NavMeshAgent agent;
    protected Transform playerTarget;
    protected Animator animator;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
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
        }
    }

    protected abstract void HandleIdleState();
    protected abstract void HandleChasingState();
    protected abstract void HandleAttackingState();

    public abstract void Attack();

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AddKill();
        }
        Destroy(gameObject);
    }
}