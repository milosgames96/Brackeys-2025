using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Collectable", menuName = "Scriptable Objects/Collectable")]
public class Collectable : ScriptableObject
{
    public enum CollectableType
    {
        CRUMB,
        CHOCOLATE_FILLING,
        JAM_FILLING,
        AMMO
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
            collectableType.Equals(CollectableType.CHOCOLATE_FILLING);
    }

    public Boolean IsAmmo()
    {
        return collectableType.Equals(CollectableType.AMMO);
    }
}
