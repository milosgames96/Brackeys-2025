using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class MilkMug : Enemy
{
    public float knockbackForce = 5f;
    private float attackCooldown = 2f;
    private float nextAttackTime = 0f;
    private Rigidbody[] ragdollRigidbodies;

    protected override void Awake()
    {
        base.Awake();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        SetRagdollState(false);
    }
    protected override void HandleIdleState()
    {
        if (isDead)
        {
            currentState = State.Death;
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
        animator.SetTrigger("Attack");

        // Ragdoll will disable agent on death
        if (agent.enabled)
            agent.isStopped = true;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer > attackRange)
        {
            currentState = State.Chasing;
            return;
        }

        transform.LookAt(playerTarget);

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

        PlayerManager playerManager = playerTarget.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.TakeDamage(damage);
        }

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

}