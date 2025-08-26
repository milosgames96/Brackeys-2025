using System.Collections;
using UnityEngine;

public class MilkCarton : Enemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private Rigidbody[] ragdollRigidbodies;
    private float nextFireTime = 0f;
    private bool isDead = false;

    protected override void Awake()
    {
        base.Awake();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        SetRagdollState(false);
    }

    protected override void HandleIdleState()
    {

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= aggroRange)
        {
            currentState = State.Chasing;
        }
    }

    protected override void HandleChasingState()
    {
        agent.isStopped = false;
        agent.SetDestination(playerTarget.position);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attacking;
        }
        else if (distanceToPlayer > aggroRange)
        {
            currentState = State.Idle;
        }
    }

    protected override void HandleAttackingState()
    {
        agent.isStopped = true;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = State.Chasing;
            return;
        }

        if (Time.time >= nextFireTime)
        {
            Attack();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    public override void Attack()
    {
        if (projectilePrefab == null || firePoint == null || isDead) return;
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
        Destroy(gameObject, 10f);
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
        if (playerTarget != null && currentState == State.Attacking)
        {
            animator.SetLookAtPosition(playerTarget.position);
            animator.SetLookAtWeight(1.0f, 0.1f, 0.9f, 0.0f, 0.5f);
        }
        else
        {
            animator.SetLookAtWeight(0.0f);
        }
    }
}