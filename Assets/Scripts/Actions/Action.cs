using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a single distinct action that may be performed as part of an Ability. It is highly dependent on contextual data, specifically information on which Ability the Action is part of, and which unit is using that Ability.
/// TODO: Split this class into child classes and use inheritance.
/// </summary>
[Serializable]
public class Action
{
    // Universal attributes
    public string type;
    public int chance = 100;

    // Optional attributes
    public UnitQuery recipientQuery;
    public List<StatusEffectApplier> effects = new List<StatusEffectApplier>();

    /// <summary>
    /// Execute this Action within the context of the unit who is using it and the associated Ability instance.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ability"></param>
    public void Execute(Unit user, ActiveAbility ability)  // TODO: Accept passives as well
    {
        List<Unit> recipients = new List<Unit>();

        // If a unit query is provided, execute it first to convert it into recipients; this is required by all Action types except for PerformAttacks that require a target tile.
        if (recipientQuery != null)
        {
            recipients = recipientQuery.Search(user);
        }

        // Perform the action
        switch (type)
        {
            case "Attack":
                if (ability.Data.requiredInput == InputType.TargetEnemyTile)  // Attack requires player-selected target tile
                {
                    GameManager.instance.Attack(ability.Data.attackData);   
                }
                else  // Attack has a fixed target selection method. This requires recipients to be found either by a unit query or accessing the results of a previous Action.
                {
                    GameManager.instance.Attack(recipients, ability.Data.attackData);
                }
                break;

            case "ApplyStatusEffects":
                GameManager.instance.ApplyStatusEffects(user, recipients, effects);
                break;

            default:
                break;
        }
    }
}