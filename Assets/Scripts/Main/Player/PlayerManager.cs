using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerHUD playerHUD;
    public PlayerUpgradeFactory playerUpgradeFactory;
    public PlayerMovement playerMovement;
    public PlayerBob playerBob;
    public PlayerProfile playerProfileTemplate;
    private List<PlayerProfileModifier> playerProfileModifiers;
    [HideInInspector]
    public bool isAlive;
    private PlayerProfile playerProfile;
    private PlayerInventory playerInventory = new PlayerInventory();
    private WeaponManager weaponManager;
    public GameObject weaponContainer;
    public GameObject chamberEnterText;
    public GameObject ropeEnterText;

    private bool isNearChamber;
    private bool isNearRope;
    private GameObject chamberObject;
    private GameObject ropeObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isAlive = true;
        playerProfileModifiers = new List<PlayerProfileModifier>() { };
        playerProfile = Instantiate(playerProfileTemplate);
        playerMovement.playerProfile = playerProfile;
        playerMovement.playerBob = playerBob;
        playerMovement.playerManager = this;
        weaponManager = GetComponent<WeaponManager>();
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
        playerHUD.DisplayAmmo(playerInventory.GetAmmo(), playerProfile.maxAmmo);
        chamberEnterText.SetActive(isNearChamber);
        ropeEnterText.SetActive(isNearRope);
        if (playerProfile.health <= 0 && isAlive)
        {
            Die();
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            weaponManager.NextWeapon();
        }
        else if (scroll < 0f)
        {
            weaponManager.PreviousWeapon();
        }
        if (Input.GetMouseButton(0))
        {
            weaponManager.PullTheTrigger(WeaponShotCallback, playerInventory.GetAmmo());
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isNearChamber)
            {
                EnterChamber();
                isNearChamber = false;
            }
            else if (isNearRope)
            {
                EnterRope();
                isNearRope = false;
            }

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
        switch (other.tag)
        {
            case "PickUp":
                PickUp pickUp = other.gameObject.GetComponent<PickUp>();
                playerInventory.InsertCollectable(pickUp.collectable);
                if (pickUp.collectable.IsWeapon())
                {
                    weaponManager.AddWeapon(pickUp.collectable);
                }
                playerHUD.NotifyPickUp(pickUp.collectable);
                Destroy(other.gameObject);
                break;
            case "UpgradeChamber":
                isNearChamber = true;
                chamberObject = other.gameObject;
                break;
            case "Rope":
                isNearRope = true;
                ropeObject = other.gameObject;
                break;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "UpgradeChamber":
                isNearChamber = false;
                break;
            case "Rope":
                isNearRope = false;
                break;
        }
    }

    private void EnterChamber()
    {
        playerUpgradeFactory.Display(playerInventory.GetFillings(), playerInventory.GetUpgrades(), playerProfile, chamberObject, ChamberDoneAndApplyCallback);
        ChamberController chamberController = chamberObject.GetComponent<ChamberController>();
        chamberController.isPlayerInside = true;
        isNearChamber = false;
        chamberEnterText.SetActive(false);
        HideWhileInChamber();
    }

    private void ExitChamber()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(true);
        playerMovement.ResetMovement();
    }
    private void EnterRope()
    {
        RopeController ropeController = ropeObject.GetComponentInParent<RopeController>();
        ropeController.EnterRope(gameObject, ExitChamber);
        ropeEnterText.SetActive(false);
        playerMovement.ResetMovement();
        HideWhileInChamber();
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

    private void ChamberDoneAndApplyCallback(PlayerProfileModifier playerProfileModifier, List<Collectable> consumedCollectables)
    {
        ExitChamber();
        AddPlayerProfileMoidifer(playerProfileModifier);
        foreach (Collectable collectable in consumedCollectables)
        {
            playerInventory.RemoveCollectable(collectable);
        }
    }

    private void WeaponShotCallback(float ammoUsed)
    {
        playerInventory.RemoveCollectable(Collectable.CollectableType.AMMO, (int)ammoUsed);
    }
}
