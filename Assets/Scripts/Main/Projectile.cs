using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("DEV Tweaks")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 3f;

    public GameObject hitEffectPrefab;
    //public GameObject defaultEffectPrefab;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile hit: {other.gameObject.name}");
        
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, transform.rotation);
            }
        }

        // It's too much splatter

        //else
        //{
        //    if (defaultEffectPrefab != null)
        //    {
        //        Instantiate(defaultEffectPrefab, transform.position, transform.rotation);
        //    }
        //}

        Destroy(gameObject);
    }
}