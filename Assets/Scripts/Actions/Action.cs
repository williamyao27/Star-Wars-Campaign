using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a single distinct action that may be performed as part of an Ability. It is highly dependent on contextual data, specifically information on which Ability the Action is part of, and which unit is using that Ability.
/// </summary>
[Serializable]
public class Action
{
    // Universal attributes
    public ActionType type;
    public int chance = 100;

    // Optional attributes
    public UnitGroup candidates;
    public string candidatesFromResult;
    public int resultIndex;
    public UnitQuery candidateQuery;
    public string list;
    public List<StatusEffectApplier> effects = new List<StatusEffectApplier>();
    public float amount;

    /// <summary>
    /// Execute this Action within the context of the unit who is using it and the associated Ability instance.
    /// </summary>
    /// <param name="user">The unit who is executing this Action.</param>
    /// <param name="ability">The Ability instance which this Action is a part of.</param>
    /// <param name="previousResults">The results of previous Actions executed by this Ability.</param>
    public ActionResult Execute(Unit user, ActiveAbility ability, List<ActionResult> previousResults)
    {
        // Generate result object and connect it to the Event Manager
        ActionResult result = new ActionResult();
        EventManager.instance.CurrentResult = result;

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
            else if (candidatesFromResult != null)
            {
                recipients = previousResults[resultIndex].Get<List<Unit>>(candidatesFromResult) ?? recipients;
            }

            // Filter recipients (in either case) by query; if a candidate group is not provided and the recipients from the previous result is null, the list is empty by this point
            if (candidateQuery != null)
            {
                recipients = candidateQuery.FilterList(recipients);
            }

            // Perform the action
            switch (type)
            {
                case ActionType.Attack:
                    // Attack requires player-selected target tile
                    if (ability.Data.requiredInput == InputType.TargetEnemyTile)
                    {
                        GameManager.instance.Attack(user, ability.Data.attackData);   
                    }

                    // Attack has a fixed target selection method. This requires recipients to be found either by a unit query or accessing the results of a previous Action.
                    else
                    {
                        GameManager.instance.Attack(user, recipients, ability.Data.attackData);
                    }
                    break;

                case ActionType.AddStatusEffects:
                    GameManager.instance.AddStatusEffects(user, recipients, effects);
                    break;

                case ActionType.RegenerateHealth:
                    GameManager.instance.RegenerateHealth(user, recipients, amount);
                    break;

                case ActionType.RegenerateTurnMeter:
                    GameManager.instance.RegenerateTurnMeter(user, recipients, amount);
                    break;

                default:
                    break;
            }
        }

        EventManager.instance.CurrentResult = null;  // Detach from Event Manager
        return result;
    }
}

public enum ActionType
{
    Attack,
    AddStatusEffects,
    RegenerateHealth,
    RemoveHealth,
    RegenerateTurnMeter,
    RemoveTurnMeter
}