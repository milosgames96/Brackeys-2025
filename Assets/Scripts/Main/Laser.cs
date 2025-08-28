using UnityEngine;

public class Laser : MonoBehaviour
{
    public float damagePerSecond = 40f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager player = other.GetComponentInParent<PlayerManager>();
            if (player != null)
            {
                // Deal damage scaled by the time between frames
                player.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }
}