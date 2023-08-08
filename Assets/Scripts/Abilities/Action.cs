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
    public ActionType Type { get; set; }
    public int chance = 100;
    public List<EventTrigger> triggers = new List<EventTrigger>();
    public UnitGroup recipientGroup;
    public string recipientFromContext;
    public UnitQuery recipientQuery;

    // Attack
    public AttackData attackData { get; set; }
    
    // Status Effect adding
    public List<StatusEffectApplier> effectsToAdd = new List<StatusEffectApplier>();
    
    // Status Effect clearing
    public List<string> effectsToRemove = new List<string>();
    public bool natural;

    // Regeneration
    public float amount;

    /// <summary>
    /// Execute this Action within the context of the unit who is using it and the parent Action.
    /// </summary>
    /// <param name="user">The unit who is executing this Action.</param>
    /// <param name="ctx">The result of the parent Action, if this is a follow-up Action.</param>
    public void Execute(Unit user, Context ctx = null)
    {
        // Enable follow-up action triggers
        foreach (EventTrigger trigger in triggers)
        {
            trigger.Enable(user);
        }

        // Check if this Action should occur based on its chance
        if (UnityEngine.Random.Range(0, 100) < chance)
        {
            List<Unit> recipients = new List<Unit>();

            // If a recipient group is provided, use it
            if (recipientGroup != UnitGroup.None)
            {
                recipients = user.GetGroup(recipientGroup);
            }
            
            // Otherwise, if a recipient to use from the parent event context is provided and it is not null, use that. Since events are only associated with a source and target unit, recipients will be a single-item list.
            else if (recipientFromContext != null)
            {
                Unit recipient = ctx.Get<Unit>(recipientFromContext);
                if (recipient != null)
                {
                    recipients = new List<Unit>{ recipient };
                }
            }

            // Filter recipients (in either case) by query; note that it may already be empty
            if (recipientQuery != null)
            {
                recipients = recipientQuery.FilterList(recipients);
            }

            // Perform the action
            switch (Type)
            {
                case ActionType.Attack:
                    GameManager.instance.Attack(user, attackData);
                    break;

                case ActionType.AddStatusEffects:
                    GameManager.instance.AddStatusEffects(user, recipients, effectsToAdd);
                    break;

                case ActionType.RemoveStatusEffects:
                    GameManager.instance.RemoveStatusEffects(user, recipients, effectsToRemove, natural);
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

        // Disable follow-up action triggers
        foreach (EventTrigger trigger in triggers)
        {
            trigger.Disable();
        }

    }
}

public enum ActionType
{
    Attack,
    AddStatusEffects,
    RemoveStatusEffects,
    RegenerateHealth,
    RegenerateTurnMeter
}