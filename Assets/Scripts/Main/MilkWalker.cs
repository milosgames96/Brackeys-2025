using UnityEngine;
using UnityEngine.AI;

public class MilkWalker : Enemy
{
    private Transform playerTarget;
    private NavMeshAgent agent;
    private float attackCooldown = 2f;
    private float nextAttackTime = 0f;

    [Header("DEV Tweaks")]
    public float attackRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on MilkWalker.");
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);

            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    public override void Attack()
    {
        if (playerTarget == null) return;

        PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            Debug.Log("MilkWalker attacks");
            playerHealth.TakeDamage(damage);
        }
    }
}
