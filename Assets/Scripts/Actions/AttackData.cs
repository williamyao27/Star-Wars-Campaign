using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This struct is a data container that stores all of the effects and target selection details associated with an attack.
/// </summary>
[Serializable]
public class AttackData
{
    public float offense = 1f;
    public float[,] pattern;
    public DamageType damageType;
    public float damage;
    public int accuracy;
    public float armorPenetration;
    public int critChance;
    public float critDamage;
    public int range;
    public LineOfFire lineOfFire;
    public List<LineOfFireModifier> lineOfFireModifiers;
    public Precision precision;
    public List<Terrain> targetableTerrains;
    public List<AttackModifier> modifiers = new List<AttackModifier>();

    /// <summary>
    /// Apply all relevant modifiers to the attack data via addition. Note that in contrast to the equivalent method in Stats, this performs only a shallow copy of the base stats as the only modifiable attack data components are value types.
    /// </summary>
    /// <param name="attacker">The unit using this attack.</param>
    /// <param name="target">The unit receiving this attack.</param>
    /// <returns>The current attack data based on modifications from the Status Effects.</returns>
    public AttackData ApplyModifiers(Unit attacker, Unit target)
    {
        AttackData modifiedData = (AttackData)this.MemberwiseClone();

        // Add the attack modifier from each Status Effect
        foreach (StatusEffect effect in attacker.StatusEffects)
        {
            AddModifier(modifiedData, effect.Data.attackModifier);
        }

        // Add the attack modifier from all modifiers
        foreach (AttackModifier modifier in modifiers)
        {
            // If the modifier includes a target query condition and it fails, do nothing
            if (modifier.targetQuery != null && !(modifier.targetQuery.EvaluateQuery(target)))
            {
                continue;
            }
            else  // Otherwise, add the attack modifier
            {
                AddModifier(modifiedData, modifier);
            }
        }

        return modifiedData;
    }

    /// <summary>
    /// Add all attack stats from the given attack modifier to the given attack data.
    /// </summary>
    /// <param name="data">The given attack data.</param>
    /// <param name="modifier">The given attack modifier.</param>
    public void AddModifier(AttackData data, AttackModifier modifier)
    {
        data.offense += modifier.offense;
        data.damage += modifier.damage;
        data.accuracy += modifier.accuracy;
        data.armorPenetration += modifier.armorPenetration;
        data.critChance += modifier.critChance;
        data.critDamage += modifier.critDamage;
    }
}

public enum DamageType
{
    Physical,
    Special
}

public enum Precision
{
    Strict,
    Area
}

public enum LineOfFire
{
    Contact,
    Direct,
    Indirect
}

public enum LineOfFireModifier
{
    Fixed,
    Rear
}