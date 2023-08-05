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

    public ActiveAbility(ActiveAbilityData data)
    {
        Data = data;
        
        // Start cooldown if needed, otherwise set to 0
        Cooldown = Data.startOnCooldown ? Data.maxCooldown : 0;

        // If this Ability is an Attack, initialize Offense to 1 (as the default is 0 if not specified)
        if (Data.attackData.stats != null)
        {
            Data.attackData.stats.offense = 1f;
        }
    }

    /// <summary>
    /// Executes the Ability's actions from the perspective of the unit user.
    /// </summary>
    /// <param name="user">The unit using this Ability.</param>
    public void Execute(Unit user)
    {
        // Begin cooldown
        Cooldown = Data.maxCooldown;

        // Track results of each Action
        List<ActionResult> results = new List<ActionResult>();

        // Execute actions using this Ability instance, the unit user, and the results of previous Actions as context
        foreach (Action action in Data.actions)
        {
            results.Add(action.Execute(user, this, results));
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

/// <summary>
/// This class represents an instance of a unit's Passive Ability.
/// </summary>
public class PassiveAbility
{
    public PassiveAbilityData Data { get; set; }

    public PassiveAbility(PassiveAbilityData data)
    {
        Data = data;
    }
}