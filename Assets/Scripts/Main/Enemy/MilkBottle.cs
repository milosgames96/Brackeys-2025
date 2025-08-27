using System.Collections;
using UnityEngine;

public class MilkBottle : Enemy
{
    public float knockbackForce = 5f;
    public GameObject explosionPrefab;
    public float animationRange; // It's special range to trigger Attack animation, since bottle is kamikaze
    public float animationDelay; // Animation length is 0.42 currently, subject to change

    private bool isPrimed = false;

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

        agent.isStopped = false;
        agent.SetDestination(playerTarget.position);

        animator.SetBool("Running", true);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= animationRange)
        {
            PrimingState();
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
        Attack();
    }
    protected override void HandleDeathState()
    {
        // Explosion VFX will come here
    }
    public override void Attack()
    {
        if (playerTarget == null) return;

        PlayerManager playerManager = playerTarget.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.TakeDamage(damage);
            Die();
        }

        Rigidbody playerRb = playerTarget.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockbackDirection = (playerTarget.position - transform.position).normalized;
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }

    protected override void Die()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (GameManager.instance != null && !isDead)
        {
            GameManager.instance.AddKill();
        }
        isDead = true;

        // No delay because kamikaze
        Destroy(gameObject);
    }

    private void PrimingState()
    {
        if (!isPrimed)
        {
            isPrimed = true;
            animator.SetTrigger("Attack");
            StartCoroutine(PrimeSuccess(animationDelay));
        }
    }

    IEnumerator PrimeSuccess(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentState = State.Attacking;
    }

}
