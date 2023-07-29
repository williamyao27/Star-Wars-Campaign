using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This struct is a data container that stores all of the effects, including damage, associated with an attack. It does not describe any specifications how the target tile for the attack, if needed, may be selected.
/// </summary>
[Serializable]
public struct AttackEffects
{
    public float[,] pattern;
    public float damage;
    public DamageType damageType;
    public int accuracy;
    public float armorPen;
    public int critChance;
    public float critDamage;
    public float suppression;
    public Precision precision;
}

/// <summary>
/// This struct is a data container that stores all information regarding how an attack may select the target tile.
/// </summary>
[Serializable]
public struct TargetTileSelector
{
    public int range;
    public LineOfFire lineOfFire;
    public List<LineOfFireModifier> lineOfFireModifiers;
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