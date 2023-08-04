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

    public StatusEffect(string name, int duration)
    {
        Data = DataManager.instance.GetStatusEffectData(name);
        Duration = duration;
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