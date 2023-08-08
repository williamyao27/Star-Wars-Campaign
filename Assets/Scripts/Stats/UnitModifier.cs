using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This struct is a data container that describes how a unit's stats should be modified based on various queries.
/// </summary>
[Serializable]
public struct UnitModifier
{
    public Stats statsBonus;
    public AttackStats attackBonus;
    public UnitGroup forEvery;
    public UnitQuery forEveryQuery;

    /// <summary>
    /// Apply this modifier's bonus to the given Stats.
    /// </summary>
    /// <param name="baseStats">The original Stats.</param>
    /// <param name="source">The unit from whose perspective this modifier's queries should be evaluated.</param>
    public void ApplyStatsBonus(Stats baseStats, Unit source)
    {
        int multiplier = 1;  // Default multiplier for the modifier is 1

        // If a "for every" query is provided, compute the multiplier based on that
        if (forEvery != UnitGroup.None)
        {
            multiplier = CountQuery(source);
        }
        
        // Add the statsBonus onto to the given base stats
        baseStats.AddStats(statsBonus, multiplier);
    }

    /// <summary>
    /// Apply this modifier's bonus to the given Attack Stats.
    /// </summary>
    /// <param name="baseStats">The original Attack Stats.</param>
    /// <param name="source">The unit from whose perspective this modifier's queries should be evaluated.</param>
    public void ApplyAttackBonus(AttackStats baseStats, Unit source)
    {
        int multiplier = 1;

        if (forEvery != UnitGroup.None)
        {
            multiplier = CountQuery(source);
        }
        
        baseStats.AddStats(attackBonus, multiplier);
    }

    /// <summary>
    /// Counts the number of units that satisfy the query associated with this modifier.
    /// </summary>
    /// <param name="source">The unit from whose perspective this query should be evaluated.</param>
    /// <returns>The number of units satisfying the query.</returns>
    private int CountQuery(Unit source)
    {
        List<Unit> units = source.GetGroup(forEvery);
        forEveryQuery.FilterList(units);
        return units.Count;
    }
}