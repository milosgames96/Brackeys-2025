using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerHUD playerHUD;
    public PlayerUpgradeFactory playerUpgradeFactory;
    public PlayerMovement playerMovement;
    public PlayerProfile playerProfileTemplate;
    private List<PlayerProfileModifier> playerProfileModifiers;
    [HideInInspector]
    public bool isAlive;
    private PlayerProfile playerProfile;
    private PlayerInventory playerInventory = new PlayerInventory();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.isAlive = true;
        this.playerProfileModifiers = new List<PlayerProfileModifier>() { };
        this.playerProfile = Instantiate(playerProfileTemplate);
        this.playerMovement.playerProfile = this.playerProfile;
        this.playerUpgradeFactory.Init(playerProfile);
        this.playerUpgradeFactory.PopulateFillingSliders(new List<Collectable>() {
            new Collectable(Collectable.CollectableType.CRUMB, 123),
            new Collectable(Collectable.CollectableType.CHOCOLATE_FILLING, 6),
            new Collectable(Collectable.CollectableType.JAM_FILLING, 4) });
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = playerProfileModifiers.Count - 1; i >= 0; i--)
        {
            PlayerProfileModifier playerProfileModifier = playerProfileModifiers[i];
            if (playerProfileModifier.isActive)
            {
                playerProfileModifier.Update();
            }
            else
            {
                playerProfileModifiers.RemoveAt(i);
            }
        }
        playerHUD.DisplayHealth(playerProfile.health);
        if (playerProfile.health <= 0 && isAlive)
        {
            Die();
        }
    }

    public void AddPlayerProfileMoidifer(PlayerProfileModifier modifier)
    {
        playerProfileModifiers.Add(modifier);
        modifier.Apply(playerProfile);
    }

    public void TakeDamage(float amount)
    {
        playerProfile.health = Mathf.Max(playerProfile.health -= amount, 0);
    }

    private void Die()
    {
        this.isAlive = false;
        Debug.Log("Rip bozo 💀");
        playerHUD.DisplayDeathScreen();
        // Pause the game
        Time.timeScale = 0f;
        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
