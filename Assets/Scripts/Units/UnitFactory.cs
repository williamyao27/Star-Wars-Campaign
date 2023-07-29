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
    /// <param name="allies">The list of allies the unit should reference.</param>
    /// <param name="enemies">The list of enemies the unit should reference.</param>
    /// <returns>The new unit instance.</returns>
    public Unit CreateUnit(string name, int teamNumber, int row, int col, List<Unit> allies, List<Unit> enemies)
    {
        // Instantiate new GO for unit
        Unit unitGO = Instantiate(unitPrefab);
        unitGO.name = name;

        // Load base data for new unit
        string json = System.IO.File.ReadAllText($"Assets/Resources/Unit Data/{name}.json");
        UnitData unitData = JsonConvert.DeserializeObject<UnitData>(json);

        // Set scripts for new unit
        Unit newUnit = unitGO.GetComponent<Unit>();
        newUnit.BaseData = unitData;
        newUnit.Allies = allies;
        newUnit.Enemies = enemies;

        // Link unit to its tile
        GridManager.instance.ConnectUnitToTile(newUnit, teamNumber, row, col);

        return newUnit;
    }
}
