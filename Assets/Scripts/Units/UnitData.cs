using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  
/// <summary>
/// This struct is a data container that stores all base stats and abilities associated with a particular definition of a unit.
/// </summary>
[Serializable]
public struct UnitData
{
    public string name;
    public string description;
    public string source;
    public Type type;
    public Terrain terrain;
    public Role role;
    public List<string> tags;
    public bool cover;
    public Stats stats;
    public List<ActiveAbilityData> activeAbilities;
    public List<PassiveAbilityData> passiveAbilities;
}

public enum Type
{
    Infantry,
    Vehicle,
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
