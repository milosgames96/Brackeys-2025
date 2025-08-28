using System.Collections;
using UnityEngine;


// Boss ain't gonna inherit from Enemy
// It will be drastically different
public class Boss : MonoBehaviour
{
    private enum BossPhase
    {
        Phase1,
        Phase2,
        Phase3,
        Defeated
    }
    private BossPhase currentPhase;
    private bool isAwake = false;
    private Transform playerTarget;

    [Header("Boss Stats")]
    public float maxHealth = 3000f;
    private float currentHealth;
    private bool isTransitioning = false;

    [Header("Butt Slam Attack")]
    public float slamRiseHeight = 20f;
    public float slamRiseSpeed = 15f;
    public float slamFallSpeed = 40f;
    public float shockwaveRadius = 30f;
    public float slamDamage = 50f;
    public GameObject shockwaveVFX; // Will probably implement later

    void Start()
    {
        currentHealth = maxHealth;
        currentPhase = BossPhase.Phase1;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
        }
    }

    void Update()
    {
        if (isTransitioning || currentPhase == BossPhase.Defeated || !isAwake)
        {
            return;
        }

        // Always face the player once awakened
        if (playerTarget != null)
        {
            Vector3 lookPosition = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);
            transform.LookAt(lookPosition);
        }

        // The state machine for what the boss does in each phase
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                DoPhase1Behavior();
                break;
            case BossPhase.Phase2:
                DoPhase2Behavior();
                break;
            case BossPhase.Phase3:
                DoPhase3Behavior();
                break;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isTransitioning || currentPhase == BossPhase.Defeated) return;

        currentHealth -= amount;

        Debug.Log("Boss took " + amount + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            CheckForPhaseTransition();
        }
    }

    private void CheckForPhaseTransition()
    {
        // P1 to P2
        if (currentPhase == BossPhase.Phase1 && currentHealth <= maxHealth * 0.66f)
        {
            StartCoroutine(TransitionToPhase(BossPhase.Phase2));
        }
        // P2 to P3
        else if (currentPhase == BossPhase.Phase2 && currentHealth <= maxHealth * 0.33f)
        {
            StartCoroutine(TransitionToPhase(BossPhase.Phase3));
        }
    }

    private IEnumerator TransitionToPhase(BossPhase nextPhase)
    {
        isTransitioning = true;
        currentPhase = nextPhase;

        Debug.Log("Transitioning to " + nextPhase.ToString() + ". Performing BUTT SLAM!");
        
        // Butt slamma logic here

        yield return new WaitForSeconds(1.0f); // Placeholder for future animation timing

        Debug.Log("Transition complete. Now in " + nextPhase.ToString());
        isTransitioning = false;
    }

    private void Die()
    {
        currentPhase = BossPhase.Defeated;
        Debug.Log("Boss has been defeated!");
        Destroy(gameObject, 2f);
    }

    // PLACEHOLDERS
    private void DoPhase1Behavior()
    {
    }
    private void DoPhase2Behavior()
    {
    }
    private void DoPhase3Behavior()
    {
    }

    public void WakeUp()
    {
        if (isAwake) return;

        isAwake = true;
        Debug.Log("Whomst've awakened the ancient one");
    }
}