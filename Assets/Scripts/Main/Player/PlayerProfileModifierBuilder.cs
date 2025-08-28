using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Analytics;

public class PlayerProfileModifierBuilder
{

    private List<PlayerProfileModifier.ValueModifier> JamBaseModifiers = new List<PlayerProfileModifier.ValueModifier>()
    {
        new PlayerProfileModifier.ValueModifier(PlayerProfileModifier.ValueModifier.ValueModifierType.PERCENTAGE, PlayerProfileModifier.ValueModifier.Field.SPEED, 4),
        new PlayerProfileModifier.ValueModifier(PlayerProfileModifier.ValueModifier.ValueModifierType.PERCENTAGE, PlayerProfileModifier.ValueModifier.Field.JUMP, 2),
        new PlayerProfileModifier.ValueModifier(PlayerProfileModifier.ValueModifier.ValueModifierType.FLAT, PlayerProfileModifier.ValueModifier.Field.HEALTH, -3)
    };
    private List<PlayerProfileModifier.ValueModifier> ChocolateBaseModifiers = new List<PlayerProfileModifier.ValueModifier>()
    {
        new PlayerProfileModifier.ValueModifier(PlayerProfileModifier.ValueModifier.ValueModifierType.FLAT, PlayerProfileModifier.ValueModifier.Field.HEALTH, 4)
    };
    private List<PlayerProfileModifier.ValueModifier> HoleBaseModifiers = new List<PlayerProfileModifier.ValueModifier>()
    {
        new PlayerProfileModifier.ValueModifier(PlayerProfileModifier.ValueModifier.ValueModifierType.FLAT, PlayerProfileModifier.ValueModifier.Field.HEALTH, -30)
    };

    private PlayerProfileModifier playerProfileModifier;
    private Dictionary<Collectable.CollectableType, int> fillings;
    private Dictionary<Collectable.CollectableType, int> upgrades;

    public PlayerProfileModifierBuilder()
    {
        playerProfileModifier = ScriptableObject.CreateInstance<PlayerProfileModifier>();
        fillings = new Dictionary<Collectable.CollectableType, int>();
        upgrades = new Dictionary<Collectable.CollectableType, int>();
    }

    public PlayerProfileModifierBuilder AddFilling(Collectable collectable, int amount)
    {
        if (collectable.IsFilling())
        {
            fillings[collectable.collectableType] = amount;
        }
        UpdateValueModifiers();
        return this;
    }

    public PlayerProfileModifierBuilder RemoveFilling(Collectable collectable)
    {
        if (collectable.IsFilling())
        {
            fillings.Remove(collectable.collectableType);
        }
        UpdateValueModifiers();
        return this;
    }

    public PlayerProfileModifierBuilder AddUpgrade(Collectable collectable, int amount)
    {
        if (collectable.IsUpgrade())
        {
            upgrades[collectable.collectableType] = amount;
        }
        UpdateValueModifiers();
        return this;
    }

    public PlayerProfileModifierBuilder RemoveUpgrade(Collectable collectable)
    {
        if (collectable.IsUpgrade())
        {
            upgrades.Remove(collectable.collectableType);
        }
        UpdateValueModifiers();
        return this;
    }

    public PlayerProfileModifier Peek()
    {
        return playerProfileModifier;
    }

    public PlayerProfileModifier Build()
    {
        PlayerProfileModifier completedPlayerProfileModifier = playerProfileModifier;
        playerProfileModifier = ScriptableObject.CreateInstance<PlayerProfileModifier>();
        return completedPlayerProfileModifier;
    }

    private void UpdateValueModifiers()
    {
        //fillings
        List<PlayerProfileModifier.ValueModifier> fillingModifiers = fillings
            .Select(pair => ParseFillingModifiers(pair.Key, pair.Value))
            .SelectMany(inner => inner)
            .ToList();
        //upgrades
        List<PlayerProfileModifier.ValueModifier> upgradeModifiers = upgrades
            .Select(pair => ParseUpgradeModifiers(pair.Key, pair.Value))
            .SelectMany(inner => inner)
            .ToList();
        //all
        List<PlayerProfileModifier.ValueModifier> allModifiers = new List<PlayerProfileModifier.ValueModifier>();
        allModifiers.AddRange(fillingModifiers);
        allModifiers.AddRange(upgradeModifiers);
        //group and collect
        playerProfileModifier.valueModifiers = allModifiers
            .GroupBy(mod => new Tuple<PlayerProfileModifier.ValueModifier.Field, PlayerProfileModifier.ValueModifier.ValueModifierType>(mod.field, mod.type))
            .Select(entry =>
            {
                PlayerProfileModifier.ValueModifier collectedValueModifier = new PlayerProfileModifier.ValueModifier();
                collectedValueModifier.field = entry.Key.Item1;
                collectedValueModifier.type = entry.Key.Item2;
                collectedValueModifier.value = entry.Sum(mod => mod.value);
                return collectedValueModifier;
            })
            .ToList();
    }

    private List<PlayerProfileModifier.ValueModifier> ParseFillingModifiers(Collectable.CollectableType collectableType, int amount)
    {
        switch (collectableType)
        {
            case Collectable.CollectableType.JAM_FILLING:
                return ApplyAmountToBaseModifiers(JamBaseModifiers, amount);
            case Collectable.CollectableType.CHOCOLATE_FILLING:
                return ApplyAmountToBaseModifiers(ChocolateBaseModifiers, amount);
            default:
                return new List<PlayerProfileModifier.ValueModifier>();

        }
    }

    private List<PlayerProfileModifier.ValueModifier> ParseUpgradeModifiers(Collectable.CollectableType collectableType, int amount)
    {
        switch (collectableType)
        {
            case Collectable.CollectableType.HOLES_UPGRADE:
                return ApplyAmountToBaseModifiers(HoleBaseModifiers, amount);
            default:
                return new List<PlayerProfileModifier.ValueModifier>();

        }
    }

    private List<PlayerProfileModifier.ValueModifier> ApplyAmountToBaseModifiers(List<PlayerProfileModifier.ValueModifier> valueModifiers, float amount)
    {
        return valueModifiers
            .Select(mod =>
            {
                PlayerProfileModifier.ValueModifier applied = new PlayerProfileModifier.ValueModifier(mod);
                applied.value *= amount;
                return applied;
            })
            .ToList();
    }
}
