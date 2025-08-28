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
    public float animationDelay = 0.5f; // Tweaking animation
    public float attackSpreadRadius = 1.5f; // A little sway to our blobs

    // This is like "fire rate" for blob bursts
    public float burstShotDelay = 0.2f;

    private float nextFireTime = 0f;

    protected override void HandleIdleState()
    {
        if (isDead)
        {
            currentState = State.Death;
        }

        if (agent.enabled)
        {
            agent.isStopped = true;
        }

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

        if (agent.enabled)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);

        }


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

        if (agent.enabled)
        {
            agent.isStopped = true;
        }

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
        //animator.SetTrigger("Death");
    }

    protected override void Die()
    {
        if (!isDead)
        {
            animator.SetTrigger("Death");

            // Disable agent so we don't get that postmortem sliding
            if (agent != null)
            {
                agent.enabled = false;
            }

            // Resize collider on death so we don't go through texture
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new Vector3(boxCollider.center.x, boxCollider.center.y, 0.5f);
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, 3f);

        }

        if (GameManager.instance != null && !isDead)
        {
            GameManager.instance.AddKill();
        }
        isDead = true;

        // NO GAMEOBJECT DESTRUCTION
        // We want it to stay as cover
    }
    public override void Attack()
    {
        animator.SetTrigger("Attack");
        

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        if (projectilePrefab == null || firePoint == null || isDead) return;
        StartCoroutine(InstantiateBlob(animationDelay));
    }

    IEnumerator InstantiateBlob(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < shotsInBurst; i++)
        {
            if (projectilePrefab != null && firePoint != null && playerTarget != null)
            {
                Vector3 startPoint = firePoint.position;

                Vector2 randomOffset = Random.insideUnitCircle * attackSpreadRadius;
                Vector3 targetPoint = playerTarget.position + new Vector3(randomOffset.x, 0, randomOffset.y);

                GameObject blobObject = Instantiate(projectilePrefab, startPoint, Quaternion.identity);
                MilkBlob blobScript = blobObject.GetComponent<MilkBlob>();

                if (blobScript != null)
                {
                    blobScript.InitializeForArc(targetPoint, projectileFlightTime);
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