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

    public List<Collectable> GetUpgrades()
    {
        return collectables
            .Select(entry => new Collectable(entry.Key, entry.Value))
            .Where(col => col.IsUpgrade())
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

    public void InsertCollectable(Collectable.CollectableType collectableType, int amount)
    {
        if (collectables.ContainsKey(collectableType))
        {
            collectables[collectableType] += amount;
        }
        else
        {
            collectables.Add(collectableType, amount);
        }
    }

    public void RemoveCollectable(Collectable collectable)
    {
        if (collectables.ContainsKey(collectable.collectableType))
        {
            collectables[collectable.collectableType] = Math.Max(0, collectables[collectable.collectableType] - collectable.amount);
            if (collectables[collectable.collectableType] <= 0)
            {
                collectables.Remove(collectable.collectableType);
            }
        }
    }

    public void RemoveCollectable(Collectable.CollectableType collectableType, int amount)
    {
        if (collectables.ContainsKey(collectableType))
        {
            collectables[collectableType] = Math.Max(0, collectables[collectableType] - amount);
            if (collectables[collectableType] <= 0)
            {
                collectables.Remove(collectableType);
            }
        }
    }
}
