using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  
/// <summary>
/// This struct is a data container that stores all properties related to a type of Status Effect.
/// </summary>
[Serializable]
public struct StatusEffectData
{
    public string name;
    public string description;
    public string picture;
    public StatusEffectType type;
    public List<UnitModifier> modifiers;
    public State stateModifier;
    public List<PassiveEventTrigger> triggers;
    public StatusEffectExpiry expiry;
    public bool stackable;
    public bool clearable;
    public bool copyable;
    public bool anonymous;  // Whether the Status Effect should be hidden, i.e. not a "true" Status Effect
}

public enum StatusEffectType
{
    Buff,
    Debuff
}

public enum StatusEffectExpiry
{
    Duration,
    Indefinite
}