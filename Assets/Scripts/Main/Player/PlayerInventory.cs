using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerInventory
{
    private Dictionary<Collectable.CollectableType, int> collectables = new Dictionary<Collectable.CollectableType, int>();
    private int ammo;

    public void insertCollectable(Collectable collectable)
    {
        if (collectables.ContainsKey(collectable.collectableType))
        {
            collectables[collectable.collectableType] += collectable.amount;
        }
        else
        {
            collectables.Add(collectable.collectableType, collectable.amount);
        }
    }

    public void removeCollectable(Collectable collectable)
    {
        if (collectables.ContainsKey(collectable.collectableType))
        {
            collectables[collectable.collectableType] = Math.Max(0, collectables[collectable.collectableType] - collectable.amount);
        }
    }
}
