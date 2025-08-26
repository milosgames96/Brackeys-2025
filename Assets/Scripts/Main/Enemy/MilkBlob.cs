using UnityEngine;

public class MilkBlob : Projectile
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        Debug.Log("MilkBlob launched");
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
