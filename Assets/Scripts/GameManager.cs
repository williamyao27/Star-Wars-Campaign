using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// This class is a singleton manager that tracks and controls all units and events in the battle.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    // Game state
    public List<Unit> AllUnits
    {
        get
        {
            return Team0.Concat(Team1).ToList();
        }
    }
    public List<Unit> Team0 { get; set; } = new List<Unit>();
    public List<Unit> Team1 { get; set; } = new List<Unit>();
    private List<Unit> turnReadyUnits = new List<Unit>();
    private Unit currentTurnUnit;
    private ActiveAbility currentSelectedAbility;
    public InputType? CurrentRequiredInput { get; set; }

    // Input
    private int targetTileTeam;
    private int targetTileRow;
    private int targetTileCol;

    private void Start()
    {
        #region TENTATIVE - should NOT create team here
        AddUnit("Stormtrooper", 0, 0, 0);  // Fill team 0 with Stormtrooper
        AddUnit("AnakinSkywalkerYoung", 1, 1, 0);  // Fill team 1 with Anakin Skywalker (Young)
        #endregion
        
        Application.targetFrameRate = 60;  // FPS = 60; choose a target that allows Turn Meter generation to be perceivable
        StartBattle();
    }

    private void Update()
    {
        RunTurnRoutine();
    }

    /// <summary>
    /// Add a new unit instance to the battle.
    /// </summary>
    /// <param name="name">The name of the new unit.</param>
    /// <param name="teamNumber">The team to which this unit belongs.</param>
    /// <param name="x">The x-position of the unit.</param>
    /// <param name="y">The y-position of the unit.</param>
    private void AddUnit(string name, int teamNumber, int row, int col)
    {
        List<Unit> teamToAdd = (teamNumber == 0) ? Team0 : Team1;
        teamToAdd.Add(UnitFactory.instance.CreateUnit(name, teamNumber, row, col));  // Append to the proper team
    }

    /// <summary>
    /// Initializes state for all battle elements.
    /// </summary>
    private void StartBattle()
    {
        // Initialize all units
        foreach (Unit unit in AllUnits)
        {
            unit.Initialize();
        }
    }

    /// <summary>
    /// Runs the standard turn routine of generating Turn Meters and prompting unit turns when they are ready.
    /// </summary>
    private void RunTurnRoutine()
    {
        // If no turns are ongoing...
        if (currentTurnUnit == null)
        {
            // ...and if there are units who are ready to take a turn, randomly choose one to begin their turn
            if (turnReadyUnits.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, turnReadyUnits.Count);
                currentTurnUnit = turnReadyUnits[randomIndex];
                turnReadyUnits.Remove(currentTurnUnit);
                currentTurnUnit.BeginTurn();
            }
            // ...otherwise, wait for units to generate Turn Meter until ready
            else
            {
                GenerateTurnMeterUntilTurnReady();
            }
        }
    }

    /// <summary>
    /// Generates natural Turn Meter for all units in battle.
    /// </summary>
    private void GenerateTurnMeterUntilTurnReady()
    {
        foreach (Unit unit in AllUnits)
        {
            if (unit.GenerateTurnMeterFromSpeed())
            {
                AddTurnReadyUnit(unit);
            }
        }
    }

    /// <summary>
    /// Adds a new unit to the list of units ready to take their turn.
    /// </summary>
    /// <param name="unit">The unit that is ready.</param>
    private void AddTurnReadyUnit(Unit unit)
    {
        turnReadyUnits.Add(unit);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void UseAbility(Unit user, ActiveAbility ability, bool endTurn)
    {
        ability.Execute();

        if (user == currentTurnUnit && endTurn)
        {
            // End the current turn
            currentTurnUnit.EndTurn();
            currentTurnUnit = null;

            // Hide Abilities and other input selection from player
            currentSelectedAbility = null;
            CurrentRequiredInput = null;
            AbilityPaletteManager.instance.HideAbilities();
            GridManager.instance.HideTargetableTiles();
        }
    }

    /// <summary>
    /// Calls the unit whose turn it currently is to use the selected Ability and then end their turn. If the given Ability requires further input, do not use it; instead, prepare for further input.
    /// </summary>
    /// <param name="ability"></param>
    public void SelectAbility(ActiveAbility ability)
    {
        // If further input is needed, simply store a reference to the given Ability as the currently selected one
        if (ability.BaseData.requiredInput != null)
        {
            currentSelectedAbility = ability;  // Store a reference to this Ability so it can be recalled when further input is made

            // Track the input type required and prepare the necessary input prompts
            switch (ability.BaseData.requiredInput)
            {
                case InputType.TargetTile:
                    CurrentRequiredInput = InputType.TargetTile;
                    GridManager.instance.ShowTargetableTiles(currentTurnUnit, ability.BaseData.targetTileSelector);
                    break;
                default:
                    // Should not reach here
                    break;
            }
        }
        // Otherwise, directly use the Ability
        else
        {
            UseAbility(currentTurnUnit, ability, true);
        }
    }

    #region Ability Inputs

    /// <summary>
    /// Sets the target tile for the currently selected Ability, then uses it and ends the current turn.
    /// </summary>
    /// <param name="teamNumber"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void SelectTargetTile(int teamNumber, int row, int col)
    {
        // Set target tile details
        targetTileTeam = teamNumber;
        targetTileRow = row;
        targetTileCol = col;

        UseAbility(currentTurnUnit, currentSelectedAbility, true);
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attack"></param>
    public void Attack(AttackEffects attackEffects)
    {
        // Determine list of targets
        List<Tuple<Unit, float>> targetWeights = GridManager.instance.EvaluateAttackPattern(attackEffects.pattern, targetTileTeam, targetTileRow, targetTileCol);

        // Evaluate attack against each target separately
        foreach (Tuple<Unit, float> targetWeight in targetWeights)
        {
            Unit target = targetWeight.Item1;
            float weight = targetWeight.Item2;
            target.ReceiveAttack(attackEffects, weight);
        }
    }

    #region Queries

    #endregion
}
