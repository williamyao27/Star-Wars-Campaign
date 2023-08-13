using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This struct stores all information related to a Passive Ability.
/// </summary>
[Serializable]
public struct PassiveAbilityData
{
    public string name;
    public string description;

    public List<UnitModifier> modifiers;
    public List<PassiveEventTrigger> triggers;
}