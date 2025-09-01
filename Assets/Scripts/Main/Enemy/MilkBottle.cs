using System.Collections;
using UnityEngine;

public class MilkBottle : Enemy
{
    public float knockbackForce = 5f;

    public float animationRange; // It's special range to trigger Attack animation, since bottle is kamikaze
    public float animationDelay; // Animation length is 0.42 currently, subject to change

    [Header("Player kills the enemy.")]
    public GameObject explosionPrefab;

    [Header("Kamikaze")]
    public GameObject deathPrefab;



    [Header("Audio")]
    public AudioClip kamikazeSound;
    public AudioSource loopingAudioSource;
    public AudioSource oneShotAudioSource;

    private bool isPrimed = false;

    protected override void Update()
    {
        base.Update();
        HandleRollingSound();
    }
    protected override void HandleIdleState(bool canSeePlayer)
    {
        if (isDead)
        {
            currentState = State.Death;
        }
        if (agent.enabled)
            agent.isStopped = true;

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

        if (agent.enabled)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);
        }

        animator.SetBool("Running", true);

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= animationRange)
        {
            PrimingState();
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

        if (agent.enabled)
            agent.isStopped = true;

        Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);
        transform.LookAt(targetPosition);

        //transform.LookAt(playerTarget);

        Attack();
    }
    protected override void HandleDeathState()
    {
    }

    public override void Attack()
    {
        if (playerTarget == null) return;

        PlayerManager playerManager = playerTarget.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.TakeDamage(damage);
            Kamikaze();
        }

        Rigidbody playerRb = playerTarget.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 knockbackDirection = (playerTarget.position - transform.position).normalized;
            playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }
    }

    // New prefab new sound, no kill counter increase
    private void Kamikaze()
    {
        if (deathPrefab != null)
        {
            Instantiate(deathPrefab, transform.position, Quaternion.identity);
        }

        if (kamikazeSound!=null)
        {
            Play2DSound(kamikazeSound);
        }

        isDead = true;

        Destroy(gameObject);
    }
    // Regular death
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with " + collision.gameObject.name);
    }

    private void HandleRollingSound()
    {
        bool isRunning = animator.GetBool("Running");

        if (isRunning && !loopingAudioSource.isPlaying)
        {
            loopingAudioSource.Play();
        }
        else if (!isRunning && loopingAudioSource.isPlaying)
        {
            loopingAudioSource.Stop();
        }
    }

}
