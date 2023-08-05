using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This struct is a data container that describes how an attack's stats should be modified based on various queries.
/// </summary>
[Serializable]
public struct AttackModifier
{
    public AttackStats attackBonus;
    public UnitQuery sourceQuery;
    public UnitQuery targetQuery;

    /// <summary>
    /// Apply this modifier's bonus to the given Attack Stats.
    /// </summary>
    /// <param name="baseStats">The original Attack Stats.</param>
    /// <param name="source">The unit from whose perspective this modifier's queries should be evaluated.</param>
    /// <param name="target">The unit receiving the attack.</param>
    public void ApplyAttackBonus(AttackStats baseStats, Unit source, Unit target)
    {
        if (sourceQuery != null && !sourceQuery.EvaluateQuery(source))
        {
            return;
        }

        if (targetQuery != null && !targetQuery.EvaluateQuery(target))
        {
            return;
        }
        
        // Add the attackBonus onto to the given base attack stats
        baseStats.AddStats(attackBonus, 1);
    }
}