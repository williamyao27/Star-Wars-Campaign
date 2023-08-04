using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a singleton manager that handles 
/// </summary>
public class EventManager : Singleton<EventManager>
{
    public ActionResult CurrentResult { get; set; }

    public void Damage(Unit source, Unit recipient, float amount)
    {
        CurrentResult.Append("damaged", recipient);
        CurrentResult.Add("totalDamage", amount);

        Debug.Log($"{recipient.Data.name} received {amount} Damage from {source.Data.name}.");
    }

    public void CriticalHit(Unit source, Unit recipient)
    {
        CurrentResult.Append("criticallyHit", recipient);

        Debug.Log($"{recipient.Data.name} received a Critical Hit from {source.Data.name}.");
    }

    public void Evasion(Unit source, Unit recipient)
    {
        CurrentResult.Append("evaded", recipient);

        Debug.Log($"{recipient.Data.name} Evaded an attack from {source.Data.name}.");
    }

    public void Buff(Unit source, Unit recipient, StatusEffectApplier effectApplier)
    {
        CurrentResult.Append("buffed", recipient);

        Debug.Log($"{recipient.Data.name} was granted {effectApplier.name} for {effectApplier.duration} turns by {source.Data.name}.");
    }

    public void Debuff(Unit source, Unit recipient, StatusEffectApplier effectApplier)
    {
        CurrentResult.Append("debuffed", recipient);

        Debug.Log($"{recipient.Data.name} was inflicted with {effectApplier.name} for {effectApplier.duration} turns by {source.Data.name}.");
    }

    public void Resist(Unit source, Unit recipient, StatusEffectApplier effectApplier)
    {
        CurrentResult.Append("resisted", recipient);

        Debug.Log($"{recipient.Data.name} Resisted {effectApplier.name} from {source.Data.name}.");
    }
}
