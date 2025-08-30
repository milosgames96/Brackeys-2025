using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponProfile", menuName = "Scriptable Objects/WeaponProfile")]
public class WeaponProfile : ScriptableObject
{
    [Serializable]
    public enum WeaponType
    {
        RANGED,
        MELEE
    }
    public WeaponType weaponType;
    public float baseDamage;
    public float critChance;
    public float accuracy;
    public float shotsPerSecond;
    public float ammoConsumption;
    public GameObject weaponPrefab;
    public AudioClip shootSound;
}
