using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Collectable", menuName = "Scriptable Objects/Collectable")]
public class Collectable : ScriptableObject
{
    [Serializable]
    public enum CollectableType
    {
        CRUMB,
        CHOCOLATE_FILLING,
        JAM_FILLING,
        YOGURT_FILLING,
        CHOCOLATE_UPGRADE,
        HOLES_UPGRADE,
        AMMO,
        PISTOL_WEAPON,
        GUN_WEAPON,
        SHOTGUN_WEAPON,
        BAT_WEAPON
    }
    public CollectableType collectableType;
    public int amount;

    public Collectable(CollectableType collectableType, int amount)
    {
        this.collectableType = collectableType;
        this.amount = amount;
    }

    public Boolean IsFilling()
    {
        return collectableType.Equals(CollectableType.JAM_FILLING) ||
            collectableType.Equals(CollectableType.CHOCOLATE_FILLING) ||
            collectableType.Equals(CollectableType.YOGURT_FILLING);
    }

    public Boolean IsUpgrade()
    {
        return collectableType.Equals(CollectableType.CHOCOLATE_UPGRADE) ||
            collectableType.Equals(CollectableType.HOLES_UPGRADE);
    }

    public Boolean IsAmmo()
    {
        return collectableType.Equals(CollectableType.AMMO);
    }

    public Boolean IsWeapon()
    {
        return collectableType.Equals(CollectableType.PISTOL_WEAPON) ||
            collectableType.Equals(CollectableType.GUN_WEAPON) ||
            collectableType.Equals(CollectableType.SHOTGUN_WEAPON) ||
            collectableType.Equals(CollectableType.BAT_WEAPON);
    }
}
