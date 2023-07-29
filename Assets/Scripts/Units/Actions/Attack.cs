using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This struct is a data container that stores all information regarding how an attack may select targets. It does not describe any effects received by the targets.
/// </summary>
[Serializable]
public struct AttackSelector
{
    public float[,] pattern;
    public int range;
    public LineOfFire lineOfFire;
    public List<LineOfFireModifier> lineOfFireModifiers;
    public Precision precision;
}

/// <summary>
/// This struct is a data container that stores all of the effects, including damage, associated with an attack.
/// </summary>
[Serializable]
public struct AttackEffects
{
    public float damage;
    public DamageType damageType;
    public int accuracy;
    public float armorPen;
    public int critChance;
    public float critDamage;
    public float suppression;
}

public enum DamageType
{
    Physical,
    Special
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
    Back
}

public enum Precision
{
    Strict,
    Area
}