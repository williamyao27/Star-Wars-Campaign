using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class describes what Actions will trigger after a certain Event takes place. It is used to create follow-up actions within the scope of a single manually-executed action, so by assumption all the relevant parties are described in the Context object passed by the Event Manager.
/// </summary>
[Serializable]
public class EventTrigger
{
    public List<EventType> onEvents;
    [SerializeReference] public List<Action> actions;
    protected Unit user;  // The unit from whose perspective the event is evaluated and actions are executed

    /// <summary>
    /// Subscribe this event trigger to the Event Manager.
    /// </summary>
    /// <param name="user">The unit that this event trigger belongs to.</param>
    public void Enable(Unit user)
    {
        this.user = user;

        foreach (EventType type in onEvents)
        {
            switch (type)
            {
                case EventType.TurnBegin:
                    EventManager.instance.OnTurnBegin += Execute;
                    break;

                case EventType.TurnEnd:
                    EventManager.instance.OnTurnEnd += Execute;
                    break;

                case EventType.Damage:
                    EventManager.instance.OnDamage += Execute;
                    break;

                case EventType.CriticalHit:
                    EventManager.instance.OnCriticalHit += Execute;
                    break;

                case EventType.Evasion:
                    EventManager.instance.OnEvasion += Execute;
                    break;

                case EventType.Buff:
                    EventManager.instance.OnBuff += Execute;
                    break;

                case EventType.Debuff:
                    EventManager.instance.OnDebuff += Execute;
                    break;

                case EventType.Resist:
                    EventManager.instance.OnResist += Execute;
                    break;

                case EventType.BuffClear:
                    EventManager.instance.OnBuffClear += Execute;
                    break;

                case EventType.DebuffClear:
                    EventManager.instance.OnDebuffClear += Execute;
                    break;

                case EventType.HalfHealth:
                    EventManager.instance.OnHalfHealth += Execute;
                    break;

                case EventType.Defeat:
                    EventManager.instance.OnDefeat += Execute;
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Unsubscribe this event trigger from the Event Manager.
    /// </summary>
    public void Disable()
    {
        user = null;

        foreach (EventType type in onEvents)
        {
            switch (type)
            {
                case EventType.TurnBegin:
                    EventManager.instance.OnTurnBegin -= Execute;
                    break;

                case EventType.TurnEnd:
                    EventManager.instance.OnTurnEnd -= Execute;
                    break;

                case EventType.Damage:
                    EventManager.instance.OnDamage -= Execute;
                    break;

                case EventType.Evasion:
                    EventManager.instance.OnEvasion -= Execute;
                    break;

                case EventType.CriticalHit:
                    EventManager.instance.OnCriticalHit -= Execute;
                    break;

                case EventType.Buff:
                    EventManager.instance.OnBuff -= Execute;
                    break;

                case EventType.Debuff:
                    EventManager.instance.OnDebuff -= Execute;
                    break;

                case EventType.Resist:
                    EventManager.instance.OnResist -= Execute;
                    break;

                case EventType.BuffClear:
                    EventManager.instance.OnBuffClear -= Execute;
                    break;

                case EventType.DebuffClear:
                    EventManager.instance.OnDebuffClear -= Execute;
                    break;

                case EventType.HalfHealth:
                    EventManager.instance.OnHalfHealth -= Execute;
                    break;

                case EventType.Defeat:
                    EventManager.instance.OnDefeat -= Execute;
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Execute the actions associated with this event trigger.
    /// </summary>
    /// <param name="ctx">The context associated with the triggering event.</param>
    public virtual void Execute(Context ctx)
    {
        foreach (Action action in actions)
        {
            action.Execute(user, ctx);
        }
    }
}

/// <summary>
/// This class describes what Actions will trigger after a certain Event takes place. Since Passive Event Triggers (unlike the base class above) listen for events throughout the game and not only during the execution of some parent Action, this subclass features more source and recipient queries to filter out all events of the required type that involve irrelevant units.
/// </summary>
public class PassiveEventTrigger : EventTrigger
{
    public UnitGroup sourceGroup;
    public UnitGroup recipientGroup;
    public UnitQuery sourceQuery;
    public UnitQuery recipientQuery;

    /// <summary>
    /// Produce a copy of this Passive Event Trigger. This method is used to create new Passive Event Triggers to subscribe to the Event Manager as they are unit-specific and run concurrently, so a new copy has to be created for each unit instance.
    /// </summary>
    /// <returns>A copy.</returns>
    public PassiveEventTrigger Copy()
    {
        PassiveEventTrigger trigger = new PassiveEventTrigger();
        trigger.onEvents = onEvents;
        trigger.actions = new List<Action>(actions);
        trigger.user = user;
        trigger.sourceGroup = sourceGroup;
        trigger.recipientGroup = recipientGroup;
        trigger.sourceQuery = sourceQuery;
        trigger.recipientQuery = recipientQuery;
        return trigger;
    }

    /// <summary>
    /// Execute the actions associated with this event trigger if the context source and recipient (if they exist) fulfill all conditions.
    /// </summary>
    /// <param name="ctx">The context associated with the triggering event.</param>
    public override void Execute(Context ctx)
    {
        // Verify source
        Unit source = ctx.Get<Unit>("source");
        if (source != null)
        {
            if (sourceGroup != UnitGroup.None && !user.GetGroup(sourceGroup).Contains(source))
            {
                return;
            }
            if (sourceQuery != null && !sourceQuery.EvaluateQuery(source))
            {
                return;
            }
        }

        // Verify recipient
        Unit recipient = ctx.Get<Unit>("recipient");
        if (recipient != null)
        {
            if (recipientGroup != UnitGroup.None && !user.GetGroup(recipientGroup).Contains(recipient))
            {
                return;
            }
            if (recipientQuery != null && !recipientQuery.EvaluateQuery(recipient))
            {
                return;
            }
        }
        
        base.Execute(ctx);
    }
}

public enum EventType
{
    TurnBegin,
    TurnEnd,
    Damage,
    Evasion,
    CriticalHit,
    Buff,
    Debuff,
    Resist,
    BuffClear,
    DebuffClear,
    HalfHealth,
    Defeat
}