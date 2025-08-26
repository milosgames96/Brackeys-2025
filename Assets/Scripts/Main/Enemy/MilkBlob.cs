using UnityEngine;

public class MilkBlob : Projectile
{
    private bool isArcInitialized = false;
    private Vector3 initialVelocityForArc;
    // MilkCarton will call this as soon as it instantiates the blob

    private void Awake()
    {
        // Override Projectile.cs
        // Blob is a lot slower and lasts longer
        lifetime = 10f; 
    }
    public void InitializeForArc(Transform target, float flightTime)
    {
        // TRAJECTORY CALCULATION
        Vector3 displacement = target.position - transform.position;
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
        if(other.CompareTag("Enemy"))
            return;

        PlayerManager playerManager = other.GetComponentInParent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
