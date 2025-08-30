using UnityEngine;

public class MeleeArea : MonoBehaviour
{
    public float duration;
    public float damage;
    bool isTriggered;//single shot only

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
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && !isTriggered)
        {
            enemy.TakeDamage(damage);
            Debug.Log(enemy.gameObject.name + " " + damage);
            isTriggered = true;
        }
    }
}
