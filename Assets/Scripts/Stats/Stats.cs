using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores all stats for the unit that may be altered during a battle.
/// </summary>
[Serializable]
public class Stats
{
    // Core stats
    public List<Tag> tags = new List<Tag>();
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
    /// Produce a deep copy of this stats instance.
    /// </summary>
    /// <returns>A deep copy.</returns>
    private Stats DeepCopy()
    {
        Stats copiedStats = new Stats();

        // Copy value types
        copiedStats.maxHealth = maxHealth;
        copiedStats.maxArmor = maxArmor;
        copiedStats.physicalDefense = physicalDefense;
        copiedStats.specialDefense = specialDefense;
        copiedStats.speed = speed;
        copiedStats.evasion = evasion;
        copiedStats.resistance = resistance;
        copiedStats.potency = potency;
        copiedStats.healthSteal = healthSteal;
        copiedStats.healthRegen = healthRegen;
        copiedStats.counterChance = counterChance;
        copiedStats.critAvoidance = critAvoidance;

        // Deep copy list of tags
        copiedStats.tags = new List<Tag>(tags.Count);
        foreach (Tag tag in tags)
        {
            copiedStats.tags.Add(tag);
        }

        return copiedStats;
    }

    /// <summary>
    /// Apply the list of Status Effects to this set of stats via addition.
    /// </summary>
    /// <param name="statusEffects">The list of Status Effects to apply.</param>
    /// <returns>The current stats based on modifications from the Status Effects.</returns>
    public Stats ApplyModifiers(List<StatusEffect> statusEffects)
    {
        Stats modifiedStats = DeepCopy();

        foreach (StatusEffect effect in statusEffects)
        {
            // If the effect includes stats modifiers, apply them to the copied base stats
            if (effect.Data.statsModifier != null)
            {
                // Add the modifier
                modifiedStats.maxHealth += effect.Data.statsModifier.maxHealth;
                modifiedStats.maxArmor += effect.Data.statsModifier.maxArmor;
                modifiedStats.physicalDefense += effect.Data.statsModifier.physicalDefense;
                modifiedStats.specialDefense += effect.Data.statsModifier.specialDefense;
                modifiedStats.speed += effect.Data.statsModifier.speed;
                modifiedStats.evasion += effect.Data.statsModifier.evasion;
                modifiedStats.resistance += effect.Data.statsModifier.resistance;
                modifiedStats.potency += effect.Data.statsModifier.potency;
                modifiedStats.healthSteal += effect.Data.statsModifier.healthSteal;
                modifiedStats.healthRegen += effect.Data.statsModifier.healthRegen;
                modifiedStats.counterChance += effect.Data.statsModifier.counterChance;
                modifiedStats.critAvoidance += effect.Data.statsModifier.critAvoidance;

                // Floor the values
                modifiedStats.maxHealth = Mathf.Max(modifiedStats.maxHealth, 1);
                modifiedStats.maxArmor = Mathf.Max(modifiedStats.maxArmor, 0);
                modifiedStats.physicalDefense = Mathf.Max(modifiedStats.physicalDefense, 0);
                modifiedStats.specialDefense = Mathf.Max(modifiedStats.specialDefense, 0);
                modifiedStats.speed = Mathf.Max(modifiedStats.speed, 0);
                modifiedStats.evasion = Mathf.Max(modifiedStats.evasion, 0);
                modifiedStats.resistance = Mathf.Max(modifiedStats.resistance, 0);
                modifiedStats.potency = Mathf.Max(modifiedStats.potency, 0);
                modifiedStats.healthSteal = Mathf.Max(modifiedStats.healthSteal, 0);
                modifiedStats.healthRegen = Mathf.Max(modifiedStats.healthRegen, 0);
                modifiedStats.counterChance = Mathf.Max(modifiedStats.counterChance, 0);
                modifiedStats.critAvoidance = Mathf.Max(modifiedStats.critAvoidance, 0);
            }
        }

        return modifiedStats;
    }
}

// Note that the Tag enum is stored in a separate file.