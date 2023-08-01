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
    public string picture;
    public Terrain terrain;
    public Role role;
    public List<Tag> tags;
    public Stats stats;
    public List<ActiveAbilityData> activeAbilities;
    public List<PassiveAbilityData> passiveAbilities;
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

public enum Tag
{
    Village,
    Undead
}
