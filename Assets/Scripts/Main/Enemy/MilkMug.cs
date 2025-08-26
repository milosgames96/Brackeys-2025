using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MilkMug : Enemy
{
    private enum State
    {
        Idle,
        Chasing,
        Attacking
    }

    private State currentState;
    private NavMeshAgent agent;
    private Transform playerTarget;
    private Animator animator;

    // We'll use this for ragdoll manipulation
    private Rigidbody[] ragdollRigidbodies;

    public GameObject projectilePrefab;
    public Transform firePoint;

    private float nextFireTime = 0f;
    private bool isDead = false;

    [Header("DEV Tweak")]
    public float fireRate = 1f;
    public float aggroRange = 40f;
    public float attackRange = 30f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        SetRagdollState(false);
    }
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
        currentState = State.Idle;
    }

    void Update()
    {
        if (playerTarget == null) 
            return;

        switch (currentState)
        {
            case State.Idle:
                animator.SetBool("Running", false);
                animator.SetBool("Attacking", false);
                SetState();
                break;

            case State.Chasing:
                animator.SetBool("Running", true);
                animator.SetBool("Attacking", false);
                agent.isStopped = false;
                agent.SetDestination(playerTarget.position);
                
                SetState();

                break;

            case State.Attacking:

                animator.SetBool("Running", false);
                animator.SetBool("Attacking", true);

                if (agent.enabled)
                    agent.isStopped = true;

                transform.LookAt(playerTarget);

                if (Time.time >= nextFireTime)
                {
                    Attack();
                    nextFireTime = Time.time + 1f / fireRate;
                }

                SetState();
                break;
        }
    }
    private void SetState()
    {
        //Debug.Log(Equals(currentState, State.Attacking) ? "Attacking" : Equals(currentState, State.Chasing) ? "Chasing" : "Idle");

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attacking;
        }
        else if (distanceToPlayer <= aggroRange)
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Idle;
        }
    }
    private void SetRagdollState(bool state)
    {
        animator.enabled = !state;
        agent.enabled = !state;

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !state;
        }
    }

    public override void Attack()
    {
        if (projectilePrefab == null || firePoint == null || isDead) return;


        // CAREFUL WITH THIS.
        // Needs to be at a good timing, exactly when head is fully forward
        // If shoot speed or fire rate is changed, it's around 3/4 of the animation length
        StartCoroutine(InstantiateBlob(0.75f));
    }

    protected override void Die()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AddKill();
        }

        isDead = true;
        SetRagdollState(true);

        // Timer not needed to be public, so just destroy after 10 seconds
        Destroy(gameObject, 10f);
    }

    IEnumerator InstantiateBlob(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (playerTarget != null)
        {
            animator.SetLookAtPosition(playerTarget.position);
            animator.SetLookAtWeight(1.0f);
        }
        else
        {
            animator.SetLookAtWeight(0.0f);
        }
    }
}