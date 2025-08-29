using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponProfile.WeaponType weaponType;
    public GameObject projectile;
    public GameObject meleeArea;
    public Transform firePoint;
    private Animator animator;
    private Dictionary<String, List<Action>> animationListeners = new Dictionary<String, List<Action>>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForCallback();
    }

    public void Equip(Action EquippedCallback)
    {
        AddAnimationListener("Idle", EquippedCallback);
    }

    public void Unequip(Action UnequipedCallback)
    {
        animator.Play("Dequip");
        AddAnimationListener("DONE", UnequipedCallback);
    }

    public void Shoot(float damage)
    {
        if (weaponType == WeaponProfile.WeaponType.RANGED)
        {
            GameObject shotProjectileObject = Instantiate(projectile, firePoint.position, firePoint.rotation);
            Projectile shotProjectile = shotProjectileObject.GetComponent<Projectile>();
            shotProjectile.damage = damage;
        }
        else if (weaponType == WeaponProfile.WeaponType.MELEE)
        {
            GameObject shotMeleeArea = Instantiate(meleeArea, firePoint.position, firePoint.rotation);
        }
        animator.Play("Shoot");
    }

    private void CheckForCallback()
    {
        int currentAnimationState = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        foreach (String animationName in animationListeners.Keys)
        {
            if (currentAnimationState == Animator.StringToHash(animationName))
            {
                foreach (Action callback in animationListeners[animationName])
                {
                    callback();
                }
                animationListeners.Remove(animationName);
                break;
            }
        }
    }

    private void AddAnimationListener(String animationName, Action callback)
    {
        if (animationListeners.ContainsKey(animationName))
        {
            animationListeners[animationName].Add(callback);
        }
        else
        {
            animationListeners.Add(animationName, new List<Action>() { callback });
        }
    }
}
