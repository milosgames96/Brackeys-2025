using System;
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PickUp")
        {

        }
        switch (other.tag)
        {
            case "PickUp":
                PickUp pickUp = other.gameObject.GetComponent<PickUp>();
                playerInventory.InsertCollectable(pickUp.collectable);
                Destroy(other.gameObject);
                break;
            case "UpgradeChamber":
                playerUpgradeFactory.Display(playerInventory.GetFillings(), playerProfile, other.gameObject.transform.Find("ChamberCamera").gameObject, ChamberDoneAndApplyCallback);
                HideWhileInChamber();
                break;
        }
    }

    private void ExitChamber()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(true);
    }

    private void HideWhileInChamber()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.SetActive(false);
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

    private void ChamberDoneAndApplyCallback(PlayerProfileModifier playerProfileModifier)
    {
        ExitChamber();
        AddPlayerProfileMoidifer(playerProfileModifier);
    }
}
