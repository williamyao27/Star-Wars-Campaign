using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a singleton manager that handles all event broadcasts and is the centralized place to which all active event triggers are subscribed.
/// </summary>
public class EventManager : Singleton<EventManager>
{
    // Delegates
    public delegate void BattleEvent(Context ctx);

    // Events
    public event BattleEvent OnTurnBegin;
    public event BattleEvent OnTurnEnd;
    public event BattleEvent OnDamage;
    public event BattleEvent OnCriticalHit;
    public event BattleEvent OnEvasion;
    public event BattleEvent OnBuff;
    public event BattleEvent OnDebuff;
    public event BattleEvent OnResist;
    public event BattleEvent OnBuffClear;
    public event BattleEvent OnDebuffClear;
    public event BattleEvent OnHalfHealth;
    public event BattleEvent OnDefeat;

    /// <summary>
    /// Create a default Context object that stores the source of an Event. This applies to some Event types.
    /// </summary>
    /// <param name="source">The unit that caused the Event.</param>
    /// <returns>The initialized Context object.</returns>
    public Context InitializeContext(Unit source)
    {
        Context ctx = new Context();
        ctx.Set("source", source);
        return ctx;
    }

    /// <summary>
    /// Create a default Context object that stores the source and recipient of an Event. This applies to the vast majority, but not all, of Event types.
    /// </summary>
    /// <param name="source">The unit that caused the Event.</param>
    /// <param name="recipient">The unit that received the Event.</param>
    /// <returns>The initialized Context object.</returns>
    public Context InitializeContext(Unit source, Unit recipient)
    {
        Context ctx = new Context();
        ctx.Set("source", source);
        ctx.Set("recipient", recipient);
        return ctx;
    }

    public void TurnBegin(Unit source)
    {
        Context ctx = InitializeContext(source);
        Debug.Log($"{source.Data.name} started their turn.");
        OnTurnBegin?.Invoke(ctx);
    }
    
    public void TurnEnd(Unit source)
    {
        Context ctx = InitializeContext(source);
        Debug.Log($"{source.Data.name} ended their turn.");
        OnTurnEnd?.Invoke(ctx);
    }

    public void Damage(Unit source, Unit recipient, float amount)
    {
        Context ctx = InitializeContext(source, recipient);
        ctx.Set("amount", amount);
        Debug.Log($"{recipient.Data.name} received {amount} Damage from {source.Data.name}.");
        OnDamage?.Invoke(ctx);
    }

    public void Evasion(Unit source, Unit recipient)
    {
        Context ctx = InitializeContext(source, recipient);
        Debug.Log($"{recipient.Data.name} Evaded an attack from {source.Data.name}.");
        OnEvasion?.Invoke(ctx);
    }

    public void CriticalHit(Unit source, Unit recipient)
    {
        Context ctx = InitializeContext(source, recipient);
        Debug.Log($"{recipient.Data.name} received a Critical Hit from {source.Data.name}.");
        OnCriticalHit?.Invoke(ctx);
    }

    public void Buff(Unit source, Unit recipient, StatusEffectApplier effectApplier)
    {
        Context ctx = InitializeContext(source, recipient);
        ctx.Set("effect", effectApplier.name);
        Debug.Log($"{recipient.Data.name} was granted {effectApplier.name} for {effectApplier.duration} turns by {source.Data.name}.");
        OnBuff?.Invoke(ctx);
    }

    public void Debuff(Unit source, Unit recipient, StatusEffectApplier effectApplier)
    {
        Context ctx = InitializeContext(source, recipient);
        ctx.Set("effect", effectApplier.name);
        Debug.Log($"{recipient.Data.name} was inflicted with {effectApplier.name} for {effectApplier.duration} turns by {source.Data.name}.");
        OnDebuff?.Invoke(ctx);
    }

    public void Resist(Unit source, Unit recipient, StatusEffectApplier effectApplier)
    {
        Context ctx = InitializeContext(source, recipient);
        ctx.Set("effect", effectApplier.name);
        Debug.Log($"{recipient.Data.name} Resisted {effectApplier.name} from {source.Data.name}.");
        OnResist?.Invoke(ctx);
    }
    
    public void BuffClear(Unit source, Unit recipient, string effectName)
    {
        Context ctx = InitializeContext(source, recipient);
        ctx.Set("effect", effectName);
        Debug.Log($"{recipient.Data.name} was cleared of {effectName} by {source.Data.name}.");
        OnBuffClear?.Invoke(ctx);
    }

    public void DebuffClear(Unit source, Unit recipient, string effectName)
    {
        Context ctx = InitializeContext(source, recipient);
        ctx.Set("effect", effectName);
        Debug.Log($"{recipient.Data.name} was cleared of {effectName} by {source.Data.name}.");
        OnDebuffClear?.Invoke(ctx);
    }

    public void HalfHealth(Unit source, Unit recipient)
    {
        Context ctx = InitializeContext(source, recipient);
        Debug.Log($"{recipient.Data.name} was brought below half-health by {source.Data.name}.");
        OnHalfHealth?.Invoke(ctx);
    }

    public void Defeat(Unit source, Unit recipient)
    {
        Context ctx = InitializeContext(source, recipient);
        Debug.Log($"{recipient.Data.name} was defeated by {source.Data.name}.");
        OnDefeat?.Invoke(ctx);
    }
}
