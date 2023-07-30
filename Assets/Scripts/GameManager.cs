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
    public ActiveAbility CurrentSelectedAbility {get; set; }
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
    /// <param name="row">The row of the unit.</param>
    /// <param name="col">The column of the unit.</param>
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
    /// Calls the unit whose turn it currently is to use the selected Ability and then end their turn. If the given Ability requires further input, do not use it; instead, prepare for further input.
    /// </summary>
    /// <param name="ability">The Ability to use.</param>
    public void SelectAbility(ActiveAbility ability)
    {
        // If further input is needed, simply store a reference to the given Ability as the currently selected one
        if (ability.BaseData.requiredInput != null)
        {
            CloseSelectedAbility();
            CurrentSelectedAbility = ability;  // Store a reference to this Ability so it can be recalled when further input is made
            CurrentRequiredInput = ability.BaseData.requiredInput;  // Track the input type required

            // Prepare the necessary input prompts
            switch (ability.BaseData.requiredInput)
            {
                case InputType.TargetTile:
                    GridManager.instance.SetTargetableTiles(currentTurnUnit, ability.BaseData.attackData);
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
    /// <param name="teamNumber">The team to which the tile belongs.</param>
    /// <param name="row">The row of the tile.</param>
    /// <param name="col">The column of the tile.</param>
    public void SelectTargetTile(int teamNumber, int row, int col)
    {
        // Set target tile details
        targetTileTeam = teamNumber;
        targetTileRow = row;
        targetTileCol = col;

        UseAbility(currentTurnUnit, CurrentSelectedAbility, true);
    }

    /// <summary>
    /// Clears the currently selected Ability and closes all input prompts that may be associated with it.
    /// </summary>
    private void CloseSelectedAbility()
    {
        CurrentSelectedAbility = null;

        // Close input prompts
        CurrentRequiredInput = null;
        GridManager.instance.HideTargetableTiles();
        GridManager.instance.HideProjectedAttackPattern();
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public void UseAbility(Unit user, ActiveAbility ability, bool endTurn)
    {
        ability.Execute();  // TODO: Add Ability context

        // If the user is the unit whose turn it currently is, and this is flagged as a turn-ending Ability (i.e. the one move allotted to a unit per turn), end the turn.
        if (user == currentTurnUnit && endTurn)
        {
            currentTurnUnit.EndTurn();
            currentTurnUnit = null;
            CloseSelectedAbility();
            AbilityPaletteManager.instance.HideAbilities();
        }
    }

    /// <summary>
    /// Perform the given attack at the current target tile.
    /// </summary>
    /// <param name="attackData">All data related to the attack.</param>
    public void Attack(AttackData attackData)
    {
        // Determine list of targets
        List<Tuple<Unit, float>> targetWeights = GridManager.instance.EvaluateAttackPattern(attackData.pattern, targetTileTeam, targetTileRow, targetTileCol);

        // Evaluate attack against each target separately
        foreach (Tuple<Unit, float> targetWeight in targetWeights)
        {
            Unit target = targetWeight.Item1;
            float weight = targetWeight.Item2;
            target.ReceiveAttack(attackData, weight);
        }
    }

    #region Queries

    #endregion
}
