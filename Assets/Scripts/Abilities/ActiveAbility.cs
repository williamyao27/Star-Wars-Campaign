using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents an instance of a unit's Active Ability.
/// </summary>
public class ActiveAbility
{
    public ActiveAbilityData Data { get; set; }
    public int Cooldown { get; set; } = 0;

    // Get the nested attack data of the first attack Action in this Ability's list of Actions
    public AttackData NestedAttackData
    {
        get
        {
            foreach (Action action in Data.actions)
            {
                if (action.Type == ActionType.Attack)
                {
                    return action.attackData;
                }
            }
            throw new InvalidOperationException("This ability is not an attack.");
        }
    }

    public ActiveAbility(ActiveAbilityData data)
    {
        Data = data;
        
        // Start cooldown if needed, otherwise set to 0
        Cooldown = Data.startOnCooldown ? Data.maxCooldown : 0;
    }

    /// <summary>
    /// Executes the Ability's actions from the perspective of the unit user.
    /// </summary>
    /// <param name="user">The unit using this Ability.</param>
    public void Execute(Unit user)
    {
        // Begin cooldown
        Cooldown = Data.maxCooldown;

        // Execute actions using this Ability instance, the unit user, and the results of previous Actions as context
        foreach (Action action in Data.actions)
        {
            action.Execute(user);
        }
    }
    
    /// <summary>
    /// Adds the given amount of turns to the cooldown of this Ability.
    /// </summary>
    /// <param name="amount">The number of turns to change the cooldown by.</param>
    public void AddCooldown(int amount)
    {
        Cooldown += amount;
        Cooldown = Mathf.Clamp(Cooldown, 0, Data.maxCooldown);
    }
}