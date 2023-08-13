using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores all mutable stats for a unit.
/// </summary>
[Serializable]
public class Stats
{
    // Core stats
    public float maxHealth;
    public float maxArmor;
    public float physicalDefense;
    public float specialDefense;
    public float speed;
    public int evasion;
    public int resistance;
    public int potency;

    // Additional stats
    public float healthSteal;
    public float healthRegen;
    public int counterChance;
    public int critAvoidance;

    /// <summary>
    /// Add the given Stats to self.
    /// </summary>
    /// <param name="stats">The Stats object to add.</param>
    /// <param name="multiplier">The amount by which to multiply the incoming Stats.</param>
    public void AddStats(Stats stats, int multiplier)
    {
        // Add the modifier
        maxHealth += stats.maxHealth * multiplier;
        maxArmor += stats.maxArmor * multiplier;
        physicalDefense += stats.physicalDefense * multiplier;
        specialDefense += stats.specialDefense * multiplier;
        speed += stats.speed * multiplier;
        evasion += stats.evasion * multiplier;
        resistance += stats.resistance * multiplier;
        potency += stats.potency * multiplier;
        healthSteal += stats.healthSteal * multiplier;
        healthRegen += stats.healthRegen * multiplier;
        counterChance += stats.counterChance * multiplier;
        critAvoidance += stats.critAvoidance * multiplier;

        // Floor the values
        maxHealth = Mathf.Max(maxHealth, 1);
        maxArmor = Mathf.Max(maxArmor, 0);
        physicalDefense = Mathf.Max(physicalDefense, 0);
        specialDefense = Mathf.Max(specialDefense, 0);
        speed = Mathf.Max(speed, 0);
        evasion = Mathf.Max(evasion, 0);
        resistance = Mathf.Max(resistance, 0);
        potency = Mathf.Max(potency, 0);
        healthSteal = Mathf.Max(healthSteal, 0);
        healthRegen = Mathf.Max(healthRegen, 0);
        counterChance = Mathf.Max(counterChance, 0);
        critAvoidance = Mathf.Max(critAvoidance, 0);
    }
    
    /// <summary>
    /// Apply the Status Effects and Modifiers of a unit to these Stats via addition.
    /// </summary>
    /// <param name="unit">The unit whose Status Effects and Modifiers to apply.</param>
    /// <returns>The new Stats based on modifications from the Status Effects.</returns>
    public Stats ApplyModifiers(Unit unit)
    {
        Stats modifiedStats = (Stats)this.MemberwiseClone();

        // Apply modifiers from each Status Effect
        foreach (StatusEffect effect in unit.StatusEffects)
        {
            if (effect.Data.modifiers != null)
            {
                foreach (UnitModifier modifier in effect.Data.modifiers)
                {
                    if (modifier.statsBonus != null)
                    {
                        modifier.ApplyStatsBonus(modifiedStats, unit);
                    }
                }
            }
        }

        // Apply modifiers from each Passive
        foreach (PassiveAbility passive in unit.PassiveAbilities)
        {
            if (passive.Data.modifiers != null)
            {
                foreach (UnitModifier modifier in passive.Data.modifiers)
                {
                    if (modifier.statsBonus != null)
                    {
                        modifier.ApplyStatsBonus(modifiedStats, unit);
                    }
                }
            }
        }
        
        return modifiedStats;
    }
}