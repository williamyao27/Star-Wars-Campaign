using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// This class is a singleton factory that instantiates Units to load into a battle.
/// </summary>
public class UnitFactory : Singleton<UnitFactory>
{
    [SerializeField] private Unit unitPrefab;

    /// <summary>
    /// Instantiate a new instance and associated GO of a desired unit. Does not initialize instance-specific data.
    /// </summary>
    /// <param name="name">The name of the new unit.</param>
    /// <param name="teamNumber">The team to which the unit belongs. Used only to determine which side of the board the unit should be placed.</param>
    /// <param name="row">The row position of the unit.</param>
    /// <param name="col">The column position of the unit.</param>
    /// <returns>The new unit instance.</returns>
    public Unit CreateUnit(string name, int teamNumber, int row, int col)
    {
        // Instantiate new GO for unit
        Unit unitGO = Instantiate(unitPrefab);
        unitGO.name = name;

        // Load base data for new unit
        UnitData unitData = DataManager.instance.GetUnitData(name);

        // Set script for new unit
        Unit newUnit = unitGO.GetComponent<Unit>();
        newUnit.Data = unitData;
        newUnit.TeamNumber = teamNumber;
        newUnit.Row = row;
        newUnit.Col = col;

        return newUnit;
    }
}
