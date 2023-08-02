using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ActiveAbility
{
    public ActiveAbilityData Data { get; set; }
    public int Cooldown { get; set; } = 0;

    public ActiveAbility(ActiveAbilityData data)
    {
        Data = data;
        
        // Begin cooldown on initialization if needed, otherwise set to 0
        Cooldown = Data.startOnCooldown ? Data.maxCooldown : 0;
    }

    public void Execute(Unit user)
    {
        // Begin cooldown
        Cooldown = Data.maxCooldown;

        // Track results of each Action
        List<Result> results = new List<Result>();

        // Execute actions using this Ability instance, the unit user, and the results of previous Actions as context
        foreach (Action action in Data.actions)
        {
            results.Add(action.Execute(user, this, results));
        }
    }
}

/// <summary>
/// 
/// </summary>
public class PassiveAbility
{
    public PassiveAbilityData Data { get; set; }

    public PassiveAbility(PassiveAbilityData data)
    {
    }
}