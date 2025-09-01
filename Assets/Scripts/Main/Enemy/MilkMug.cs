using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class MilkMug : Enemy
{
    public float knockbackForce = 5f;
    public float attackDelay = 0.5f; // Tweak values

    private float attackCooldown = 2f;
    private float nextAttackTime = 0f;
    private Rigidbody[] ragdollRigidbodies;

    protected override void Awake()
    {
        base.Awake();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        SetRagdollState(false);
    }
    protected override void HandleIdleState(bool canSeePlayer)
    {
        if (isDead)
        {
            currentState = State.Death;
        }

        // Ragdoll will disable agent on death
        if (agent.enabled)
        {
            agent.isStopped = true;
        }

        animator.SetBool("Running", false);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= aggroRange && canSeePlayer)
        {
            currentState = State.Chasing;
        }
    }

    protected override void HandleChasingState(bool canSeePlayer)
    {
        if (isDead)
        {
            currentState = State.Death;
        }

        // Ragdoll will disable agent on death
        if (agent.enabled)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
        }

        animator.SetBool("Running", true);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attacking;
        }
        else if (distanceToPlayer > aggroRange || !canSeePlayer)
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

        // Ragdoll will disable agent on death
        if (agent.enabled)
            agent.isStopped = true;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = State.Chasing;
            return;
        }

        Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);
        transform.LookAt(targetPosition);

        //transform.LookAt(playerTarget);

        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    protected override void HandleDeathState()
    {
        SetRagdollState(true);
    }

    public override void Attack()
    {
        if (playerTarget == null) return;

        animator.SetTrigger("Attack");
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }

        StartCoroutine(PlayerInAttackRange(attackDelay));

        Rigidbody playerRb = playerTarget.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockbackDirection = (playerTarget.position - transform.position).normalized;
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
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

    IEnumerator PlayerInAttackRange(float attackDelay)
    {
        yield return new WaitForSeconds(attackDelay);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // Is player STILL in range after the delay?
        // If so , apply damage
        if (distanceToPlayer <= attackRange)
        {
            PlayerManager playerManager = playerTarget.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.TakeDamage(damage);
            }
        }
    }
}