using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This struct is a data container that stores all of the effects and target selection details associated with an attack.
/// </summary>
[Serializable]
public struct AttackData
{
    public float[,] pattern;
    public float damage;
    public DamageType damageType;
    public int accuracy;
    public float armorPen;
    public int critChance;
    public float critDamage;
    public int range;
    public LineOfFire lineOfFire;
    public List<LineOfFireModifier> lineOfFireModifiers;
    public Precision precision;
    public List<Terrain> targetableTerrains;
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