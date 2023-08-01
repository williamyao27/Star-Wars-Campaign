using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  
/// <summary>
/// This struct is a data container that stores all properties related to a type of status effect.
/// </summary>
[Serializable]
public struct StatusEffectData
{
    public string name;
    public string description;
    public string picture;
    public StatusEffectType type;
    public Stats modifiers;
    // List of on-infliction effects
    // List of passives associated
    public bool clearable;
    public bool copyable;
    public bool anonymous;  // Whether the status effect should be hidden, i.e. not a "true" status effect
}

public enum StatusEffectType
{
    Buff,
    Debuff
}