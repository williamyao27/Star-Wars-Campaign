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
    public List<Tag> tags;
    public float maxHealth;
    public float maxArmor;
    public float physicalDefense;
    public float specialDefense;
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
        copiedStats.cover = cover;
        copiedStats.healthSteal = healthSteal;
        copiedStats.healthRegen = healthRegen;
        copiedStats.counterChance = counterChance;
        copiedStats.critAvoidance = critAvoidance;

        // Deep copy list of tags
        if (tags != null)
        {
            copiedStats.tags = new List<Tag>(tags.Count);
            foreach (Tag tag in tags)
            {
                copiedStats.tags.Add(tag);
            }
        }

        return copiedStats;
    }

    /// <summary>
    /// Apply the list of Status Effects to this set of stats via addition.
    /// </summary>
    /// <param name="statusEffects">The list of Status Effects to apply.</param>
    /// <returns>The current stats based on modifications from the Status Effects.</returns>
    public Stats ApplyStatusEffects(List<StatusEffect> statusEffects)
    {
        Stats modifiedStats = DeepCopy();

        foreach (StatusEffect effect in statusEffects)
        {
            // If the effect includes stats modifiers, apply them to the copied base stats
            if (effect.Data.statsModifier != null)
            {
                // For numerical values, add the modifier. For bool values, replace with the modifier.
                modifiedStats.maxHealth += effect.Data.statsModifier.maxHealth;
                modifiedStats.maxArmor += effect.Data.statsModifier.maxArmor;
                modifiedStats.physicalDefense += effect.Data.statsModifier.physicalDefense;
                modifiedStats.specialDefense += effect.Data.statsModifier.specialDefense;
                modifiedStats.speed += effect.Data.statsModifier.speed;
                modifiedStats.evasion += effect.Data.statsModifier.evasion;
                modifiedStats.resistance += effect.Data.statsModifier.resistance;
                modifiedStats.potency += effect.Data.statsModifier.potency;
                modifiedStats.cover = effect.Data.statsModifier.cover;
                modifiedStats.healthSteal += effect.Data.statsModifier.healthSteal;
                modifiedStats.healthRegen += effect.Data.statsModifier.healthRegen;
                modifiedStats.counterChance += effect.Data.statsModifier.counterChance;
                modifiedStats.critAvoidance += effect.Data.statsModifier.critAvoidance;
            }
        }

        return modifiedStats;
    }
}

// Note that the Tag enum is stored in a separate file.