using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class CharacterAttribute
{
    #region Variables

    private float baseValue = 0;
    private AttributeTypes attributeType;
    public virtual float ModifiedValue 
    { 
        get 
        {
            if (baseValue != lastBaseValue || isDirty)
            {
                lastBaseValue = baseValue;
                modifiedValue = CalculateFinalValue();                             
                isDirty = false;
            }
            return modifiedValue;       
        }
    }
    protected bool isDirty = false;
    protected float modifiedValue;
    protected float lastBaseValue = float.MinValue;
    protected float baseValueBeforeReset = 0;
    protected readonly List<AttributeModifier> statModifiers;
    public readonly ReadOnlyCollection<AttributeModifier> StatModifiers;
    public event Action<AttributeTypes> OnStatChanged;
    public float BaseValue => baseValue;
    public AttributeTypes AttributeType => attributeType;

    #endregion

    #region General

    public CharacterAttribute(AttributeTypes AT)
    {
        statModifiers = new List<AttributeModifier>();
        StatModifiers = statModifiers.AsReadOnly();
        attributeType = AT;
    }

    public CharacterAttribute(float baseValue, AttributeTypes AT) : this(AT)
    {
        this.baseValue = baseValue;
        baseValueBeforeReset = baseValue;
    }

    public virtual void IncrementBaseValue(float val, AttributeModType modType)
    {
        float modValue = baseValue;
        if (modType == AttributeModType.Flat)
        {
            modValue = modValue + val;
        }
        else if (modType == AttributeModType.PercentAdd)
        {
            modValue = modValue * (1 + val);
        }
        baseValue = (float)Math.Round(modValue, 2);   
        OnStatChanged?.Invoke(attributeType);
    }

    public virtual void ResetValue()
    {
        baseValue = baseValueBeforeReset;
    }

    public virtual void ApplyValue()
    {
        baseValueBeforeReset = baseValue;
    }

    public virtual void AddModifier(AttributeModifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModOrder);
    }

    protected virtual int CompareModOrder(AttributeModifier a, AttributeModifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
        {
            return 1;
        }
        return 0;
    }

    public virtual bool RemoveModifier(AttributeModifier mod)
    {              
        if (statModifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;
        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].Source == source)
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }

    protected virtual float CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPctAdd = 0;
        for (int i = 0; i < statModifiers.Count; i++)
        {
            AttributeModifier mod = statModifiers[i];
            if (mod.Type == AttributeModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if (mod.Type == AttributeModType.PercentAdd)
            {
                sumPctAdd += mod.Value;
                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != AttributeModType.PercentAdd)
                {
                    finalValue *= 1 + sumPctAdd;
                    sumPctAdd = 0;
                }
            }
            else if (mod.Type == AttributeModType.PercentMult)
            {
                finalValue *= 1 + mod.Value;
            }
        }
        OnStatChanged?.Invoke(attributeType);
        return (float)Math.Round(finalValue, 2);
    }

    #endregion
}
