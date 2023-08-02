using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores a comprehensive set of information related to the result of the execution of an Action.
/// </summary>
public class Result
{
    public List<Tuple<Unit, float>> DamageByTarget { get; set; } = new List<Tuple<Unit, float>>();
    public List<Unit> CriticallyHitTargets { get; set; } = new List<Unit>();
    public List<Unit> EvadedTargets { get; set; } = new List<Unit>();

    public List<Unit> DamagedTargets
    {
        get
        {
            List<Unit> targets = new List<Unit>();
            foreach (Tuple<Unit, float> pair in DamageByTarget)
            {
                targets.Add(pair.Item1);
            }
            return targets;
        }
    }
    
    public float TotalDamage
    {
        get
        {
            float totalDamage = 0f;
            foreach (Tuple<Unit, float> pair in DamageByTarget)
            {
                totalDamage += pair.Item2;
            }
            return totalDamage;
        }
    }
}