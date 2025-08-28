using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory
{
    private Dictionary<Collectable.CollectableType, int> collectables = new Dictionary<Collectable.CollectableType, int>();

    public int GetAmmo()
    {
        return collectables.ContainsKey(Collectable.CollectableType.AMMO) ? collectables[Collectable.CollectableType.AMMO] : 0;
    }

    public List<Collectable> GetFillings()
    {
        return collectables
            .Select(entry => new Collectable(entry.Key, entry.Value))
            .Where(col => col.IsFilling())
            .ToList();
    }

    public void InsertCollectable(Collectable collectable)
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

    public void RemoveCollectable(Collectable collectable)
    {
        if (collectables.ContainsKey(collectable.collectableType))
        {
            collectables[collectable.collectableType] = Math.Max(0, collectables[collectable.collectableType] - collectable.amount);
        }
    }
}
