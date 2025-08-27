using System;
using UnityEngine;

public class Collectable
{
    public enum CollectableType
    {
        CRUMB,
        CHOCOLATE_FILLING,
        JAM_FILLING
    }
    public CollectableType collectableType { get; set; }
    public int amount { get; set; }

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
}
