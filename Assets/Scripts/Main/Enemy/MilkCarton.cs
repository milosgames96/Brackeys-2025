using System.Collections;
using UnityEngine;

public class MilkCarton : Enemy
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("DEV Tweaks")]
    public float fireCooldown = 1f;
    public float projectileFlightTime = 2f;
    public int shotsInBurst = 4;

    // This is like "fire rate" for blob bursts
    public float burstShotDelay = 0.2f;

    private float nextFireTime = 0f;

    protected override void HandleIdleState()
    {
        if (isDead)
        {
            currentState = State.Death;
        }

        agent.isStopped = true;
        animator.SetBool("Running", false);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= aggroRange)
        {
            currentState = State.Chasing;
        }
    }

    protected override void HandleChasingState()
    {
        if (isDead)
        {
            currentState = State.Death;
        }

        animator.SetBool("Running", true);
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
        if (isDead)
        {
            currentState = State.Death;
        }

        animator.SetBool("Running", false);
        agent.isStopped = true;

        transform.LookAt(playerTarget);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = State.Chasing;
            return;
        }

        if (Time.time >= nextFireTime)
        {
            Attack();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    protected override void HandleDeathState()
    {
        // Animation will come here
    }
    public override void Attack()
    {
        if (projectilePrefab == null || firePoint == null || isDead) return;
        StartCoroutine(InstantiateBlob(0.75f));
    }

    IEnumerator InstantiateBlob(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < shotsInBurst; i++)
        {
            if (projectilePrefab != null && firePoint != null && playerTarget != null)
            {
                Vector3 startPoint = firePoint.position;
                Vector3 targetPoint = playerTarget.position;

                Vector3 displacement = targetPoint - startPoint;
                Vector3 initialVelocity = (displacement / projectileFlightTime) - (Physics.gravity * projectileFlightTime / 2f);

                GameObject blobObject = Instantiate(projectilePrefab, startPoint, Quaternion.identity);
                MilkBlob blobScript = blobObject.GetComponent<MilkBlob>();
                if (blobScript != null)
                {
                    blobScript.InitializeForArc(playerTarget, projectileFlightTime);
                }
            }

            // Wait before firing the next shot in the burst
            if (i < shotsInBurst - 1)
            {
                yield return new WaitForSeconds(burstShotDelay);
            }
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