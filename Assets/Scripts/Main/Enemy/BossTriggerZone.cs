using UnityEngine;

public class BossTriggerZone : MonoBehaviour
{
    public Boss bossToWake;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the player
        if (other.CompareTag("Player"))
        {
            if (bossToWake != null)
            {
                Debug.Log("Boss triggered");
                bossToWake.WakeUp();

                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("The boss isn't assigned", this);
            }
        }
    }
}