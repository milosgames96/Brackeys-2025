using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    [Serializable]
    public class CollectableWeapon
    {
        public Collectable.CollectableType collectableType;
        public WeaponProfile weaponProfile;
    }
    public List<CollectableWeapon> collectableWeaponLookup;
    
    [HideInInspector]
    public bool isJammed;
    private List<WeaponProfile> weaponProfiles;
    private int currentWeaponIndex;
    private float shootingTimer;
    private WeaponController currentWeaponController;
    public GameObject weaponsContainer;
    private bool isFirstEquip;

    private void Start()
    {
        weaponProfiles = new List<WeaponProfile>();
        currentWeaponIndex = -1;
    }

    private void Update()
    {
        if (shootingTimer > 0)
        {
            shootingTimer -= Time.deltaTime;
        }
    }

    public void AddWeapon(Collectable collectable)
    {
        if (collectable.IsWeapon())
        {
            WeaponProfile weaponProfile = collectableWeaponLookup
                .Where(cw => cw.collectableType.Equals(collectable.collectableType))
                .First().weaponProfile;
            weaponProfiles.Add(weaponProfile);
            isFirstEquip = true;
            if (HasCurrentWeapon())
            {
                currentWeaponIndex = weaponProfiles.Count - 1;
                UnequipWeapon();
            }
            else
            {
                currentWeaponIndex = 0;
                EquipWeapon();
            }
        }
    }

    public void NextWeapon()
    {
        if (weaponProfiles.Count > 0 && weaponProfiles.Count > 1 && !isJammed)
        {
            currentWeaponIndex++;
            if (currentWeaponIndex >= weaponProfiles.Count)
            {
                currentWeaponIndex = 0;
            }
            UnequipWeapon();
        }
    }

    public void PreviousWeapon()
    {
        if (weaponProfiles.Count > 0 && weaponProfiles.Count > 1 && !isJammed)
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = weaponProfiles.Count - 1;
            }
            UnequipWeapon();
        }
    }

    public void PullTheTrigger(Action<float> ShootCallback, float currentAmmo)
    {
        if (shootingTimer <= 0 && !isJammed && HasCurrentWeapon())
        {
            if (currentAmmo >= weaponProfiles[currentWeaponIndex].ammoConsumption)
            {
                ShootWeapon();
                PrepareForShot();
                ShootCallback(weaponProfiles[currentWeaponIndex].ammoConsumption);   
            }
        }
    }

    private bool HasCurrentWeapon()
    {
        return currentWeaponIndex >= 0;
    }

    private void ShootWeapon()
    {
        currentWeaponController.Shoot(CalculateDamage());
    }

    private void EquipWeapon()
    {
        GameObject weaponControllerObject = GameObject.Instantiate(weaponProfiles[currentWeaponIndex].weaponPrefab, weaponsContainer.transform);
        currentWeaponController = weaponControllerObject.GetComponent<WeaponController>();
        currentWeaponController.Equip(EquipedCallback, isFirstEquip);
        currentWeaponController.shootSound  = weaponProfiles[currentWeaponIndex].shootSound;
        isFirstEquip = false;
    }

    private void EquipedCallback()
    {
        isJammed = false;
        PrepareForShot();
    }

    private void UnequipWeapon()
    {
        isJammed = true;
        currentWeaponController.Unequip(UnequipedCallback);
    }

    private void UnequipedCallback()
    {
        GameObject.Destroy(currentWeaponController.gameObject);
        if (HasCurrentWeapon())
        {
            EquipWeapon();
        }
    }

    private float CalculateDamage()
    {
        return weaponProfiles[currentWeaponIndex].baseDamage;
    }

    private void PrepareForShot()
    {
        WeaponProfile weaponProfile = weaponProfiles[currentWeaponIndex];
        shootingTimer = 1 / weaponProfile.shotsPerSecond;
    }
}
