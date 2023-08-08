using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents an instance of a unit's Passive Ability.
/// </summary>
public class PassiveAbility
{
    public PassiveAbilityData Data { get; set; }
    public List<PassiveEventTrigger> Triggers { get; set; } = new List<PassiveEventTrigger>();

    public PassiveAbility(PassiveAbilityData data, Unit user)
    {
        Data = data;

        // Copy and subscribe each Event Trigger template to the Event Manager
        if (Data.triggers != null)
        {
            foreach (PassiveEventTrigger triggerTemplate in Data.triggers)
            {
                PassiveEventTrigger trigger = triggerTemplate.Copy();
                trigger.Enable(user);
                Triggers.Add(trigger);
            }
        }
    }
}