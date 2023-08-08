using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  
/// <summary>
/// This class represents a single instance of a Status Effect that is experienced by a unit.
/// </summary>
public class StatusEffect
{
    public StatusEffectData Data { get; set; }
    public int Duration { get; set; }
    public List<PassiveEventTrigger> Triggers { get; set; } = new List<PassiveEventTrigger>();

    public StatusEffect(string name, int duration)
    {
        Data = DataManager.instance.GetStatusEffectData(name);
        Duration = duration;
    }

    /// <summary>
    /// Executes the on-application effects of receiving the Status Effect.
    /// </summary>
    /// <param name="user">The unit receiving the Status Effect.</param>
    public void OnApply(Unit user)
    {
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

    /// <summary>
    /// Executse the on-removal effects of losing the Status Effect.
    /// </summary>
    public void OnRemove()
    {
        foreach (PassiveEventTrigger trigger in Triggers)
        {
            trigger.Disable();
        }
    }

    /// <summary>
    /// Changes the duration of this Status Effect by the given amount.
    /// </summary>
    /// <param name="amount">The number of turns to change the duration by.</param>
    public bool ChangeDuration(int amount)
    {
        Duration += amount;
        return Duration <= 0;
    }

    /// <summary>
    /// Reduce the duration of this Status Effect by 1 turn and return whether it has expired, if applicable.
    /// </summary>
    /// <returns>Whether the Status Effect has expired and needs to be removed from the unit.</returns>
    public bool DecrementDuration()
    {
        if (Data.expiry == StatusEffectExpiry.Duration)
        {
            return ChangeDuration(-1);
        }
        // For indefinite Status Effects, do nothing
        else
        {
            return false;
        }
    }
}