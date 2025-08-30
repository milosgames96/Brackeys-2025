using UnityEngine;

public class MeleeArea : MonoBehaviour
{
    public float duration;
    public float damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration < 0)
        {
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Melee hit: {other.gameObject.name}");
        
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
