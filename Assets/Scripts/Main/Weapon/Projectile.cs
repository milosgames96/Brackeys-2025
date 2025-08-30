using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("DEV Tweaks")]
    public float speed = 20f;
    public float damage = 10f;
    public float lifetime = 3f;

    public GameObject hitEffectPrefab;

    private Rigidbody rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            return;
        }

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Boss boss = other.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
        }
        Instantiate(hitEffectPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}