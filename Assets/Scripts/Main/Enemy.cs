using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int damage;

    public virtual void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.AddKill();
        }

        Destroy(gameObject);
    }

    public abstract void Attack();
}