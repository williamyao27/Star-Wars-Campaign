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
    public float bravery;
    public int resistance;
    public int potency;
    public Cover cover;

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
        copiedStats.bravery = stats.bravery;
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
    public static Stats ApplyModifiers(Stats stats, List<Modifier> modifiers)
    {
        Stats modifiedStats = DeepCopy(stats);

        return modifiedStats;
    }

}

public enum Type
{
    Infantry,
    Vehicle,
    CapitalShip,
    Equipment,
}

public enum Terrain
{
    Ground,
    Aerial,
}

public enum Role
{
    Attacker,
    Support,
    Tank,
    Healer,
}

public enum Cover
{
    None,
    Partial,
    Full,
}

// Note that the tag enum is stored in a separate file.