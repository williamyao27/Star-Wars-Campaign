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
    /// Produce a deep copy of the given stats instance.
    /// </summary>
    /// <param name="stats">The stats instance to deep copy.</param>
    /// <returns></returns>
    private static Stats DeepCopy(Stats stats)
    {
        Stats copiedStats = new Stats();

        // Copy value types
        copiedStats.maxHealth = stats.maxHealth;
        copiedStats.maxArmor = stats.maxArmor;
        copiedStats.physicalDefense = stats.physicalDefense;
        copiedStats.specialDefense = stats.specialDefense;
        copiedStats.speed = stats.speed;
        copiedStats.evasion = stats.evasion;
        copiedStats.resistance = stats.resistance;
        copiedStats.potency = stats.potency;
        copiedStats.cover = stats.cover;
        copiedStats.healthSteal = stats.healthSteal;
        copiedStats.healthRegen = stats.healthRegen;
        copiedStats.counterChance = stats.counterChance;
        copiedStats.critAvoidance = stats.critAvoidance;

        // Deep copy list of tags
        if (stats.tags != null)
        {
            copiedStats.tags = new List<Tag>(stats.tags.Count);
            foreach (Tag tag in stats.tags)
            {
                copiedStats.tags.Add(tag);
            }
        }

        return copiedStats;
    }

    /// <summary>
    /// Apply the list of stat modifiers to a set of base stats via addition.
    /// </summary>
    /// <param name="stats">The base stats.</param>
    /// <param name="modifiers">The list of modifiers to apply.</param>
    /// <returns></returns>
    public static Stats ApplyStatusEffects(Stats stats, List<StatusEffect> statusEffects)
    {
        Stats modifiedStats = DeepCopy(stats);

        foreach (StatusEffect effect in statusEffects)
        {
            Debug.Log(effect.Data.statsModifier);

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