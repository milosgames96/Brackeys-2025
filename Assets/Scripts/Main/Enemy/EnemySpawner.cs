using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyList;
    public float spawnRadius = 20f;
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 4f;
    public float minSpawnDistanceFromPlayer = 10f;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
            NavMeshHit hit;

            // Only and ONLY if the point is on the NavMesh
            if (NavMesh.SamplePosition(randomPoint, out hit, spawnRadius, NavMesh.AllAreas))
            {
                // Ensure the spawn point is a minimum distance away from the player
                // Since sometimes they spawn behind you
                // "Nothing personal, kid"
                if (Vector3.Distance(hit.position, playerTransform.position) >= minSpawnDistanceFromPlayer)
                {
                    GameObject enemyPrefab = this.enemyList[Random.Range(0, this.enemyList.Count)];
                    Instantiate(enemyPrefab, hit.position, Quaternion.identity);
                }
            }
        }
    }
    
    // Need to see the spawn radius
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}