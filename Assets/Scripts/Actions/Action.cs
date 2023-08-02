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
    public ActionType type;
    public int chance = 100;  // The chance that this Action's effects are performed

    // Optional attributes
    public UnitQuery recipientsFromQuery;
    public string recipientsFromResult;
    public int resultIndex;
    public List<StatusEffectApplier> effects = new List<StatusEffectApplier>();

    /// <summary>
    /// Execute this Action within the context of the unit who is using it and the associated Ability instance.
    /// </summary>
    /// <param name="user">The unit who is executing this Action.</param>
    /// <param name="ability">The Ability instance which this Action is a part of.</param>
    /// <param name="previousResults">The results of previous Actions executed by this Ability.</param>
    public Result Execute(Unit user, ActiveAbility ability, List<Result> previousResults)
    {
        Result result = new Result();

        // Check if this Action should occur based on its chance
        if (UnityEngine.Random.Range(0, 100) < chance)
        {
            List<Unit> recipients = new List<Unit>();

            // If a unit query is provided, evaluate it first to convert it into recipients; this is required by all Action types except for PerformAttacks that require a target tile.
            if (recipientsFromQuery != null)
            {
                recipients = recipientsFromQuery.Search(user);
            }

            // Otherwise, if a field to use from a previous result dictionary is provided to find recipients, use that
            else if (recipientsFromResult != null)
            {
                recipients = previousResults[resultIndex].Get<List<Unit>>(recipientsFromResult);
            }

            // Perform the action
            switch (type)
            {
                case ActionType.Attack:
                    // Attack requires player-selected target tile
                    if (ability.Data.requiredInput == InputType.TargetEnemyTile)
                    {
                        GameManager.instance.Attack(ability.Data.attackData, result);   
                    }

                    // Attack has a fixed target selection method. This requires recipients to be found either by a unit query or accessing the results of a previous Action.
                    else
                    {
                        GameManager.instance.Attack(recipients, ability.Data.attackData, result);
                    }
                    break;

                case ActionType.ApplyStatusEffects:
                    GameManager.instance.ApplyStatusEffects(user, recipients, effects);
                    break;

                default:
                    break;
            }
        }

        return result;
    }
}

public enum ActionType
{
    Attack,
    ApplyStatusEffects
}