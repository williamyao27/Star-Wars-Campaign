using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores all mutable attack stats for an attack Action.
/// </summary>
public class AttackStats
{
    public float offense;
    public float damage;
    public int accuracy;
    public float armorPenetration;
    public int critChance;
    public float critDamage;

    /// <summary>
    /// Add the given Attack Stats to self.
    /// </summary>
    /// <param name="stats">The Attack Stats object to add.</param>
    /// <param name="multiplier">The amount by which to multiply the incoming Stats.</param>
    public void AddStats(AttackStats stats, int multiplier)
    {
        // Add the modifier
        offense += stats.offense * multiplier;
        damage += stats.damage * multiplier;
        accuracy += stats.accuracy * multiplier;
        armorPenetration += stats.armorPenetration * multiplier;
        critChance += stats.critChance * multiplier;
        critDamage += stats.critDamage * multiplier;

        // Floor the values
        offense = Mathf.Max(offense, 0);
        damage = Mathf.Max(damage, 0);
        accuracy = Mathf.Max(accuracy, 0);
        armorPenetration = Mathf.Clamp(armorPenetration, 0, 1);
        critChance = Mathf.Max(critChance, 0);
        critDamage = Mathf.Max(critDamage, 0);
    }
    
    /// <summary>
    /// Apply all relevant Attack Modifiers to these Attack Stats via addition.
    /// </summary>
    /// <param name="source">The unit using this attack.</param>
    /// <param name="recipient">The unit receiving this attack.</param>
    /// <param name="attackModifiers">The source- and target-based modifiers associated with this attack.</param>
    /// <returns>The new Attack Stats based on modifications from the source and recipient.</returns>
    public AttackStats ApplyModifiers(Unit source, Unit recipient, List<AttackModifier> attackModifiers)
    {
        AttackStats modifiedStats = (AttackStats)this.MemberwiseClone();

        // Apply modifiers from each Status Effect
        foreach (StatusEffect effect in source.StatusEffects)
        {
            if (effect.Data.modifiers != null)
            {
                foreach (UnitModifier modifier in effect.Data.modifiers)
                {
                    if (modifier.attackBonus != null)
                    {
                        modifier.ApplyAttackBonus(modifiedStats, source);
                    }
                }
            }
        }

        // Apply modifiers from each Passive
        foreach (PassiveAbility passive in source.PassiveAbilities)
        {
            if (passive.Data.modifiers != null)
            {
                foreach (UnitModifier modifier in passive.Data.modifiers)
                {
                    if (modifier.attackBonus != null)
                    {
                        modifier.ApplyAttackBonus(modifiedStats, source);
                    }
                }
            }
        }

        // Apply source- and target-based bonuses from each Attack Modifier
        if (attackModifiers != null)
        {
            foreach (AttackModifier modifier in attackModifiers)
            {
                modifier.ApplyAttackBonus(modifiedStats, source, recipient);
            }
        }

        return modifiedStats;
    }
}