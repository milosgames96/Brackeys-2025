using UnityEngine;
using UnityEngine.AI;

public class MilkWalker : Enemy
{
    private Transform playerTarget;

    private NavMeshAgent agent;

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
        }
    }

    public override void Attack()
    {
    }
}
