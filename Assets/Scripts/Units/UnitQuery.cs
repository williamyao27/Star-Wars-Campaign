using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class 
/// </summary>
[Serializable]
public class UnitQuery
{
    // Universal attributes
    public Candidates candidates;

    // Optional attributes
    public List<Tag> tags = new List<Tag>();

    public List<Unit> Search(Unit querier)
    {
        List<Unit> units = new List<Unit>();

        // Get basic list of candidates
        switch (candidates)
        {
            case Candidates.Self:
                units = new List<Unit>{ querier };
                break;

            case Candidates.Allies:
                units = new List<Unit>(querier.Allies);
                break;

            case Candidates.Enemies:
                units = new List<Unit>(querier.Enemies);
                break;

            case Candidates.OtherAllies:
                units = new List<Unit>(querier.Allies);
                units.Remove(querier);
                break;

            case Candidates.AllUnits:
                units = new List<Unit>(GameManager.instance.AllUnits);
                break;

            default:
                break;
        }

        // Filter units
        for (int i = units.Count - 1; i >= 0; i--)
        {
            Unit currentUnit = units[i];

            // By tags
            if (!currentUnit.HasTags(tags))
            {
                units.RemoveAt(i);
                continue;
            }
        }

        return units;
    }
}

public enum Candidates
{
    Self,
    Allies,
    Enemies,
    OtherAllies,
    AllUnits
}