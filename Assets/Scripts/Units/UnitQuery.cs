using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class 
/// </summary>
[Serializable]
public class UnitQuery
{
    public Candidates candidates;

    public List<Unit> Search(Unit querier)
    {
        List<Unit> result = new List<Unit>();

        switch (candidates)
        {
            case Candidates.Self:
                result = new List<Unit>{ querier };
                break;

            case Candidates.Allies:
                result = new List<Unit>(querier.Allies);
                break;

            case Candidates.Enemies:
                result = new List<Unit>(querier.Enemies);
                break;

            case Candidates.OtherAllies:
                result = new List<Unit>(querier.Allies);
                result.Remove(querier);
                break;

            case Candidates.AllUnits:
                result = new List<Unit>(GameManager.instance.AllUnits);
                break;

            default:
                break;
        }

        return result;
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