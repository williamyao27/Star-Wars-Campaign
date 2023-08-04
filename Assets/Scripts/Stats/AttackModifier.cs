using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This struct is a data container used to store all possibly modifiable attack stats to be added on some attack data.
/// </summary>
[Serializable]
public struct AttackModifier
{
    public float offense;
    public DamageType damageType;
    public float damage;
    public int accuracy;
    public float armorPenetration;
    public int critChance;
    public float critDamage;

    // Optional query
    public UnitQuery targetQuery;
}