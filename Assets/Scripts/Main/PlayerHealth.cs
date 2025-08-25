using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public TextMeshProUGUI healthText;
    public GameObject deathScreenUI;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();

        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
    }

    private void Die()
    {
        Debug.Log("Rip bozo 💀");

        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }

        // Pause the game
        Time.timeScale = 0f;
        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}