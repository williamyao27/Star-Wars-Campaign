using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores all stats for the unit that may be altered during a battle.
/// </summary>
[Serializable]
public struct Stats
{
    // Core stats
    public float maxHealth;
    public float maxArmor;
    public float physicalDefense;
    public float explosiveDefense;
    public float magicDefense;
    public float speed;
    public int evasion;
    public int resistance;
    public int potency;
    public bool cover;

    // Additional stats
    public float healthSteal;
    public float healthRegen;
    public int counterChance;
    public int critAvoidance;
}

/// <summary>
/// 
/// </summary>
public static class StatsModifier
{
    /// <summary>
    /// Applies the list of Status Effects to a set of base stats via addition.
    /// </summary>
    /// <param name="stats">The base stats.</param>
    /// <param name="statusEffects">The list of Status Effects to apply.</param>
    /// <returns></returns>
    public static Stats ApplyStatusEffects(Stats stats, List<StatusEffect> statusEffects)
    {
        Stats modifiedStats = stats;

        foreach (StatusEffect effect in statusEffects)
        {
                // For numerical values, add the modifier. For bool values, replace with the modifier.
                modifiedStats.maxHealth += effect.Data.modifiers.maxHealth;
                modifiedStats.maxArmor += effect.Data.modifiers.maxArmor;
                modifiedStats.physicalDefense += effect.Data.modifiers.physicalDefense;
                modifiedStats.explosiveDefense += effect.Data.modifiers.explosiveDefense;
                modifiedStats.magicDefense += effect.Data.modifiers.magicDefense;
                modifiedStats.speed += effect.Data.modifiers.speed;
                modifiedStats.evasion += effect.Data.modifiers.evasion;
                modifiedStats.resistance += effect.Data.modifiers.resistance;
                modifiedStats.potency += effect.Data.modifiers.potency;
                modifiedStats.cover = effect.Data.modifiers.cover;
                modifiedStats.healthSteal += effect.Data.modifiers.healthSteal;
                modifiedStats.healthRegen += effect.Data.modifiers.healthRegen;
                modifiedStats.counterChance += effect.Data.modifiers.counterChance;
                modifiedStats.critAvoidance += effect.Data.modifiers.critAvoidance;
        }

        return modifiedStats;
    }
}