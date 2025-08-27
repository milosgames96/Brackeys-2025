using UnityEngine;

public class MilkBlob : Projectile
{
    private bool isArcInitialized = false;
    private Vector3 initialVelocityForArc;

    public GameObject blobEffectPrefab;


    private void Awake()
    {
        // Override Projectile.cs
        // Blob is a lot slower and lasts longer
        lifetime = 10f; 
    }
    public void InitializeForArc(Vector3 target, float flightTime)
    {
        // TRAJECTORY CALCULATION
        Vector3 displacement = target - transform.position;
        initialVelocityForArc = (displacement / flightTime) - (Physics.gravity * flightTime / 2f);
        isArcInitialized = true;
    }

    protected override void Start()
    {
        // If the blobArc was correctly initialized, use the calc
        if (isArcInitialized)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = initialVelocityForArc;
            }
            Destroy(gameObject, lifetime);
        }

        // Failsafe if something goes wrong
        else
        {
            base.Start();
        }
    }
    

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") || other.CompareTag("Blob"))
            return;

        PlayerManager playerManager = other.GetComponentInParent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.TakeDamage(damage);
        }

        if (blobEffectPrefab != null)
        {
            Instantiate(blobEffectPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
