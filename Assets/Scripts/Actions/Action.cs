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
    public UnitGroup candidates;
    public string recipientsFromResult;
    public int resultIndex;
    public UnitQuery unitQuery;
    public List<StatusEffectApplier> effects = new List<StatusEffectApplier>();

    /// <summary>
    /// Execute this Action within the context of the unit who is using it and the associated Ability instance.
    /// </summary>
    /// <param name="user">The unit who is executing this Action.</param>
    /// <param name="ability">The Ability instance which this Action is a part of.</param>
    /// <param name="previousResults">The results of previous Actions executed by this Ability.</param>
    public ActionResult Execute(Unit user, ActiveAbility ability, List<ActionResult> previousResults)
    {
        ActionResult result = new ActionResult();

        // Check if this Action should occur based on its chance
        if (UnityEngine.Random.Range(0, 100) < chance)
        {
            List<Unit> recipients = new List<Unit>();

            // If a candidate group is provided, use it
            if (candidates != UnitGroup.None)
            {
                recipients = user.GetGroup(candidates);
            }
            // Otherwise, if a field to use from a previous result is provided to find recipients and it is not null, use that
            else if (recipientsFromResult != null)
            {
                recipients = previousResults[resultIndex].Get<List<Unit>>(recipientsFromResult) ?? recipients;
            }
            // Filter recipients (in either case) by unit query
            if (unitQuery != null)
            {
                recipients = unitQuery.FilterList(recipients);
            }

            // Perform the action
            switch (type)
            {
                case ActionType.Attack:
                    AttackData currentAttackData = ability.Data.attackData.ApplyStatusEffects(user.StatusEffects);  // Get current attack data

                    // Attack requires player-selected target tile
                    if (ability.Data.requiredInput == InputType.TargetEnemyTile)
                    {
                        GameManager.instance.Attack(currentAttackData, result);   
                    }

                    // Attack has a fixed target selection method. This requires recipients to be found either by a unit query or accessing the results of a previous Action.
                    else
                    {
                        GameManager.instance.Attack(recipients, currentAttackData, result);
                    }
                    break;

                case ActionType.AddStatusEffects:
                    GameManager.instance.AddStatusEffects(user, recipients, effects, result);
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
    AddStatusEffects
}