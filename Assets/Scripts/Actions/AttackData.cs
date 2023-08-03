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

    /// <summary>
    /// Apply the list of Status Effects to the attack data via addition. Note that in contrast to the equivalent method in Stats, this performs only a shallow copy as the only modifiable attack data components are value types.
    /// </summary>
    /// <param name="statusEffects">The list of Status Effects to apply.</param>
    /// <returns>The current attack data based on modifications from the Status Effects.</returns>
    public AttackData ApplyStatusEffects(List<StatusEffect> statusEffects)
    {
        AttackData modifiedData = (AttackData)this.MemberwiseClone();

        foreach (StatusEffect effect in statusEffects)
        {
            // If the effect includes attack data modifiers, apply them to the copied data
            if (effect.Data.attackModifier != null)
            {
                modifiedData.offense += effect.Data.attackModifier.offense;
                modifiedData.damage += effect.Data.attackModifier.damage;
                modifiedData.accuracy += effect.Data.attackModifier.accuracy;
                modifiedData.armorPenetration += effect.Data.attackModifier.armorPenetration;
                modifiedData.critChance += effect.Data.attackModifier.critChance;
                modifiedData.critDamage += effect.Data.attackModifier.critDamage;
            }
        }

        return modifiedData;
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