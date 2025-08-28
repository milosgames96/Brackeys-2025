using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfileModifier", menuName = "Scriptable Objects/PlayerProfileModifier")]
public class PlayerProfileModifier : ScriptableObject
{

    [System.Serializable]
    public class ValueModifier
    {
        public enum ValueModifierType
        {
            FLAT,
            PERCENTAGE
        }

        public enum Field
        {
            HEALTH,
            SPEED,
            JUMP
        }

        public ValueModifier()
        {
            
        }

        public ValueModifier(ValueModifierType type, Field field, float value)
        {
            this.type = type;
            this.field = field;
            this.value = value;
        }

        public ValueModifier(ValueModifier valueModifier)
        {
            this.type = valueModifier.type;
            this.field = valueModifier.field;
            this.value = valueModifier.value;
        }


        public ValueModifierType type;
        public Field field;
        public float value;
    }


    public bool isTimed;
    public float duration;
    public List<ValueModifier> valueModifiers;
    [HideInInspector]
    public bool isActive;
    private PlayerProfile playerProfile;
    private float remainingDuration;

    public void Apply(PlayerProfile playerProfile)
    {
        this.isActive = true;
        this.playerProfile = playerProfile;
        this.remainingDuration = duration;
        ApplyValueModifiers(CalculateModifiedValue);
    }

    public void Update()
    {
        if (isTimed)
        {
            remainingDuration -= Time.deltaTime;
            if (remainingDuration < 0 && isActive)
            {
                Remove();
            }
        }
    }

    public void Remove()
    {
        this.isActive = false;
        ApplyValueModifiers(CalculateOriginalValue);
    }

    private void ApplyValueModifiers(Func<ValueModifier, float, float> calculator)
    {
        foreach (ValueModifier valueModifier in valueModifiers)
        {
            switch (valueModifier.field)
            {
                case ValueModifier.Field.HEALTH:
                    playerProfile.health = calculator(valueModifier, playerProfile.health);
                    break;
                case ValueModifier.Field.SPEED:
                    playerProfile.maxSpeed = calculator(valueModifier, playerProfile.maxSpeed);
                    break;
                case ValueModifier.Field.JUMP:
                    playerProfile.jumpForce = calculator(valueModifier, playerProfile.jumpForce);
                    break;
            }
        }
    }

    private float CalculateModifiedValue(ValueModifier valueModifier, float baseValue)
    {
        switch (valueModifier.type)
        {
            case ValueModifier.ValueModifierType.PERCENTAGE:
                return baseValue + (baseValue / 100) * valueModifier.value;
            case ValueModifier.ValueModifierType.FLAT:
            default:
                return baseValue + valueModifier.value;
        }
    }

    private float CalculateOriginalValue(ValueModifier valueModifier, float currentValue)
    {
        switch (valueModifier.type)
        {
            case ValueModifier.ValueModifierType.PERCENTAGE:
                return currentValue / (1 + valueModifier.value / 100f);
            case ValueModifier.ValueModifierType.FLAT:
            default:
                return currentValue - valueModifier.value;
        }
    }
}
