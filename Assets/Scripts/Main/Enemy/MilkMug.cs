using UnityEngine;

public class MilkMug : Enemy
{
    public float knockbackForce = 5f;
    private float attackCooldown = 2f;
    private float nextAttackTime = 0f;

    protected override void HandleIdleState()
    {
        animator.SetBool("Running", false);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= aggroRange)
        {
            currentState = State.Chasing;
        }
    }

    protected override void HandleChasingState()
    {
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
        animator.SetBool("Running", false);
        animator.SetTrigger("Attack");

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
}