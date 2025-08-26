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

    void Start()
    {
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
                GameObject enemyPrefab = this.enemyList[Random.Range(0, this.enemyList.Count)];
                Instantiate(enemyPrefab, hit.position, Quaternion.identity);
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