using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is a query object used to determine whether a unit or group of units fulfills a set of conditions.
/// </summary>
[Serializable]
public class UnitQuery
{
    private List<Tag> tags = new List<Tag>();

    /// <summary>
    /// Evaluates whether the given unit fulfills all the conditions.
    /// </summary>
    /// <param name="unit">The unit to evaluate.</param>
    /// <returns>Whether the unit fulfills all the conditions.</returns>
    public bool EvaluateQuery(Unit unit)
    {
        // By tags
        if (!unit.HasTags(tags))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Filters the given list of units by whether they fulfill all the conditions.
    /// </summary>
    /// <param name="units">The list of units to filrter.</param>
    /// <returns>Remaining list of units that fulfill all the conditions.</returns>
    public List<Unit> FilterList(List<Unit> units)
    {
        for (int i = units.Count - 1; i >= 0; i--)
        {
            Unit currentUnit = units[i];
            if (!EvaluateQuery(currentUnit))
            {
                units.RemoveAt(i);
                continue;
            }
        }

        return units;
    }
}