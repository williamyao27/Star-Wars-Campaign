using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
  
/// <summary>
/// 
/// </summary>
public class StatusEffect
{
    public StatusEffectData Data { get; set; }
    public int Duration { get; set; }

    public StatusEffect(string name, int duration)
    {
        Data = DataManager.instance.GetStatusEffectData(name);  // Retrieve base data using name
        Duration = duration;
    }
}