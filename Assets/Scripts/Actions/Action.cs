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
    public string recipientsFromActionResult;
    public int resultIndex;
    public List<StatusEffectApplier> effects = new List<StatusEffectApplier>();

    /// <summary>
    /// Execute this Action within the context of the unit who is using it and the associated Ability instance.
    /// </summary>
    /// <param name="user">The unit who is executing this Action.</param>
    /// <param name="ability">The Ability instance which this Action is a part of.</param>
    /// <param name="previousActionResults">The results of previous Actions executed by this Ability.</param>
    public ActionResult Execute(Unit user, ActiveAbility ability, List<ActionResult> previousActionResults)
    {
        ActionResult result = new ActionResult();
        GameManager.instance.CurrentResult = result;  // Attach it to the Game manager for easy access

        // Check if this Action should occur based on its chance
        if (UnityEngine.Random.Range(0, 100) < chance)
        {
            List<Unit> recipients = new List<Unit>();

            // If a unit query is provided, evaluate it first to convert it into recipients; this is required by all Action types except for PerformAttacks that require a target tile.
            if (recipientsFromQuery != null)
            {
                recipients = recipientsFromQuery.Search(user);
            }

            // Otherwise, if a field to use from a previous result is provided to find recipients and it is not null, use that
            else if (recipientsFromActionResult != null)
            {
                recipients = previousActionResults[resultIndex].Get<List<Unit>>(recipientsFromActionResult) ?? recipients;
            }

            // Perform the action
            switch (type)
            {
                case ActionType.Attack:
                    // Attack requires player-selected target tile
                    if (ability.Data.requiredInput == InputType.TargetEnemyTile)
                    {
                        GameManager.instance.Attack(ability.Data.attackData);   
                    }

                    // Attack has a fixed target selection method. This requires recipients to be found either by a unit query or accessing the results of a previous Action.
                    else
                    {
                        GameManager.instance.Attack(recipients, ability.Data.attackData);
                    }
                    break;

                case ActionType.ApplyEffects:
                    GameManager.instance.ApplyStatusEffects(user, recipients, effects);
                    break;

                default:
                    break;
            }
        }

        GameManager.instance.CurrentResult = null;  // Detach from the Game manager
        return result;
    }
}

public enum ActionType
{
    Attack,
    ApplyEffects
}