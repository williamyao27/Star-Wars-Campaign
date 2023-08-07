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
    public delegate void DamageEvent(Context ctx);
    public delegate void CriticalHitEvent(Context ctx);
    public delegate void EvasionEvent(Context ctx);

    // Events
    public event DamageEvent OnDamage;
    public event CriticalHitEvent OnCriticalHit;
    public event EvasionEvent OnEvasion;

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

    public void Damage(Unit source, Unit recipient, float amount)
    {
        Context ctx = InitializeContext(source, recipient);
        ctx.Set("amount", amount);
        Debug.Log($"{recipient.Data.name} received {amount} Damage from {source.Data.name}.");
        OnDamage?.Invoke(ctx);
    }

    public void CriticalHit(Unit source, Unit recipient)
    {
        Context ctx = InitializeContext(source, recipient);
        Debug.Log($"{recipient.Data.name} received a Critical Hit from {source.Data.name}.");
        OnCriticalHit?.Invoke(ctx);
    }

    public void Evasion(Unit source, Unit recipient)
    {
        Context ctx = InitializeContext(source, recipient);
        Debug.Log($"{recipient.Data.name} Evaded an attack from {source.Data.name}.");
        OnEvasion?.Invoke(ctx);
    }

    public void Buff(Unit source, Unit recipient, StatusEffectApplier effectApplier)
    {
        Debug.Log($"{recipient.Data.name} was granted {effectApplier.name} for {effectApplier.duration} turns by {source.Data.name}.");
    }

    public void Debuff(Unit source, Unit recipient, StatusEffectApplier effectApplier)
    {
        Debug.Log($"{recipient.Data.name} was inflicted with {effectApplier.name} for {effectApplier.duration} turns by {source.Data.name}.");
    }

    public void Resist(Unit source, Unit recipient, StatusEffectApplier effectApplier)
    {
        // CurrentResult.Append("resisted", recipient);

        Debug.Log($"{recipient.Data.name} Resisted {effectApplier.name} from {source.Data.name}.");
    }
}
