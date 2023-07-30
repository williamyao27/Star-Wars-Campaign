using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ActiveAbility
{
    public ActiveAbilityData BaseData { get; set; }
    public int Cooldown { get; set; } = 0;

    public ActiveAbility(ActiveAbilityData baseData)
    {
        BaseData = baseData;
        
        // Begin cooldown on initialization if needed, otherwise set to 0
        Cooldown = BaseData.startOnCooldown ? BaseData.maxCooldown : 0;
    }

    public void Execute()
    {
        // Begin cooldown
        Cooldown = BaseData.maxCooldown;

        // Execute actions using this Ability instance as context
        foreach (Action action in BaseData.actions)
        {
            action.Execute(this);
        }
    }
}

/// <summary>
/// 
/// </summary>
public class PassiveAbility
{
    public PassiveAbilityData BaseData { get; set; }

    public PassiveAbility(PassiveAbilityData baseData)
    {
    }
}