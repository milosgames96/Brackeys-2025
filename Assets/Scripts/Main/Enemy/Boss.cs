using System.Collections;
using UnityEngine;


// Boss ain't gonna inherit from Enemy
// It will be drastically different
public class Boss : MonoBehaviour
{
    private enum AttackType 
    { 
        Mortar, 
        Laser, 
        BasicProjectile 
    }
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
    private float nextFireTime = 0f;

    [Header("Boss Stats")]
    public float maxHealth = 3000f;
    private float currentHealth;
    private bool isTransitioning = false;

    [Header("Combat")]
    public float fireCooldown = 4f;
    public Transform firePoint;

    [Header("Butt Slam Attack")]
    public float slamRiseHeight = 20f;
    public float slamRiseSpeed = 15f;
    public float slamFallSpeed = 40f;
    public float shockwaveRadius = 30f;
    public float slamDamage = 50f;
    public GameObject shockwaveVFX; // Will probably implement later

    [Header("Attack Prefabs")]
    public GameObject mortarProjectilePrefab;
    public GameObject basicProjectilePrefab;
    public GameObject laserPrefab;

    [Header("Mortar Attack Settings")]
    public float projectileFlightTime = 2f;
    public int shotsInBurst = 5;
    public float burstShotDelay = 0.3f;
    public float attackSpreadRadius = 3f;
    public float mortarAnimationDelay = 0.75f;

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
                HandleAttacks();
                break;
            case BossPhase.Phase2:
                HandleAttacks();
                break;
            case BossPhase.Phase3:
                HandleAttacks();
                break;
        }
    }

    private void HandleAttacks()
    {
        if (Time.time >= nextFireTime)
        {
            ChooseAndExecuteAttack();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    private void ChooseAndExecuteAttack()
    {
        AttackType chosenAttack = (AttackType)Random.Range(0, 3);
        switch (chosenAttack)
        {
            case AttackType.Mortar:
                StartCoroutine(MortarAttack());
                break;
            case AttackType.Laser:
                StartCoroutine(LaserAttack());
                break;
            case AttackType.BasicProjectile:
                StartCoroutine(BasicProjectileAttack());
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

        // Original position
        float originalY = transform.position.y;
        Vector3 upPosition = transform.position + Vector3.up * slamRiseHeight;

        // Jump up
        while (transform.position.y < upPosition.y - 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, upPosition, slamRiseSpeed * Time.deltaTime);
            yield return null;
        }

        // Slamma
        Vector3 downPosition = new Vector3(transform.position.x, originalY, transform.position.z);
        while (transform.position.y > downPosition.y + 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, downPosition, slamFallSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = downPosition;

        // Future VFX here
        if (shockwaveVFX != null)
        {
            Instantiate(shockwaveVFX, transform.position, Quaternion.identity);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceToPlayer <= shockwaveRadius)
        {
            PlayerManager player = playerTarget.GetComponent<PlayerManager>();
            if (player != null)
            {
                player.TakeDamage(slamDamage);
            }
        }

        Debug.Log("Transition complete. Next: " + nextPhase.ToString());
        isTransitioning = false;
    }

    private void Die()
    {
        currentPhase = BossPhase.Defeated;
        Debug.Log("Boss has been defeated!");
        Destroy(gameObject, 2f);
    }

    public void WakeUp()
    {
        if (isAwake) return;

        isAwake = true;
        Debug.Log("Whomst've awakened the ancient one");
    }

    private IEnumerator MortarAttack()
    {
        Debug.Log("Boss is using Mortar Attack!");
        yield return new WaitForSeconds(mortarAnimationDelay);

        for (int i = 0; i < shotsInBurst; i++)
        {
            if (mortarProjectilePrefab != null)
            {
                Vector2 randomOffset = Random.insideUnitCircle * attackSpreadRadius;
                Vector3 targetPoint = playerTarget.position + new Vector3(randomOffset.x, 0, randomOffset.y);
                GameObject blobObject = Instantiate(mortarProjectilePrefab, firePoint.position, Quaternion.identity);
                MilkBlob blobScript = blobObject.GetComponent<MilkBlob>();
                if (blobScript != null)
                {
                    // Same Milkblob arc logic as the Carton
                    blobScript.InitializeForArc(targetPoint, projectileFlightTime);
                }
            }
            if (i < shotsInBurst - 1)
            {
                yield return new WaitForSeconds(burstShotDelay);
            }
        }
    }

    private IEnumerator LaserAttack()
    {
        Debug.Log("Boss is using Laser Attack!");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator BasicProjectileAttack()
    {
        Debug.Log("Boss is using Basic Projectile Attack!");
        yield return new WaitForSeconds(0.5f);
    }
}