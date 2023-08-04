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
    private List<Unit> turnQueue = new List<Unit>();
    private Unit currentTurnUnit;

    // Input
    public ActiveAbility CurrentSelectedAbility {get; set; }
    public InputType? CurrentRequiredInput { get; set; }
    private int targetTileTeam;
    private int targetTileRow;
    private int targetTileCol;

    private void Start()
    {
        #region TENTATIVE - should NOT create team here
        AddUnit("Stormtrooper", 0, 0, 0);
        AddUnit("Stormtrooper", 0, 0, 1);
        AddUnit("Anakin Skywalker (Young)", 1, 0, 0);
        AddUnit("B2 Super Battle Droid", 1, 0, 1);
        #endregion
        
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

    #region Turn Routine

    /// <summary>
    /// Runs the standard turn routine of generating Turn Meters and prompting unit turns when they are ready.
    /// </summary>
    private void RunTurnRoutine()
    {
        // If no turns are ongoing...
        if (currentTurnUnit == null)
        {
            // And if there are units who are enqueued to take a turn, randomly choose one to begin their turn
            if (turnQueue.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, turnQueue.Count);
                
                currentTurnUnit = turnQueue[randomIndex];
                RemoveTurnReadyUnit(currentTurnUnit);
                
                if (currentTurnUnit.BeginTurn())  // Begin the current unit's turn
                {
                    // Immediately skip it (e.g. Stun)
                    EndCurrentTurn();
                }
                else
                {
                    // Otherwise, display Abilities to player
                    AbilityPaletteManager.instance.ShowAbilities(currentTurnUnit.ActiveAbilities);
                }
            }
            else
            {
                List<Unit> unitsToEnqueue = new List<Unit>();
                foreach (Unit unit in AllUnits)
                {
                    if (unit.ReadyForTurn)
                    {
                        AddTurnReadyUnit(unit);
                    }
                }

                // If there are units who are ready to take a turn, enqueue them
                if (unitsToEnqueue.Count > 0)
                {
                    turnQueue = unitsToEnqueue;
                }
                else
                {
                    // Otherwise, wait for units to generate Turn Meter until ready
                    GenerateTurnMeterUntilTurnReady();
                }
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
            unit.GenerateTurnMeterFromSpeed();
        }
    }

    /// <summary>
    /// Adds a new unit to the list of units ready to take their turn.
    /// </summary>
    /// <param name="unit">The unit that is ready.</param>
    private void AddTurnReadyUnit(Unit unit)
    {
        if (!turnQueue.Contains(unit))
        {
            turnQueue.Add(unit);
        }
    }
    
    /// <summary>
    /// Removes the given unit from the list of units ready to take their turn.
    /// </summary>
    /// <param name="unit">The unit to remove.</param>
    private void RemoveTurnReadyUnit(Unit unit)
    {
        if (turnQueue.Contains(unit))
        {
            turnQueue.Remove(unit);
        }
    }
    
    /// <summary>
    /// Ends the current turn and resets any game state variables and input prompts that depended on the turn.
    /// </summary>
    private void EndCurrentTurn()
    {
        currentTurnUnit.EndTurn();
        currentTurnUnit = null;
        CloseSelectedAbility();
        AbilityPaletteManager.instance.HideAbilities();
    }

    #endregion

    #region Ability Inputs

    /// <summary>
    /// Calls the unit whose turn it currently is to use the selected Ability and then end their turn. If the given Ability requires further input, do not use it; instead, prepare for further input.
    /// </summary>
    /// <param name="ability">The Ability to use.</param>
    public void SelectAbility(ActiveAbility ability)
    {
        // If further input is needed, simply store a reference to the given Ability as the currently selected one
        if (ability.Data.requiredInput != null)
        {
            CloseSelectedAbility();
            CurrentSelectedAbility = ability;  // Store a reference to this Ability so it can be recalled when further input is made
            CurrentRequiredInput = ability.Data.requiredInput;  // Track the input type required

            // Prepare the necessary input prompts
            switch (ability.Data.requiredInput)
            {
                case InputType.TargetEnemyTile:
                    GridManager.instance.SetTargetableTiles(currentTurnUnit, ability.Data.attackData);
                    break;
                    
                default:
                    break;
            }
        }
        // Otherwise, directly use the Ability
        else
        {
            UseAbility(currentTurnUnit, ability, true);
        }
    }

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
        GridManager.instance.HideVisualizedAttackPattern();
    }

    /// <summary>
    /// 
    /// </summary>
    public void UseAbility(Unit user, ActiveAbility ability, bool endTurn)
    {
        ability.Execute(user);

        // If the user is the unit whose turn it currently is, and this is flagged as a turn-ending Ability (i.e. the one move allotted to a unit per turn), end the turn.
        if (user == currentTurnUnit && endTurn)
        {
            EndCurrentTurn();
        }
    }

    #endregion

    #region Health

    /// <summary>
    /// Regenerates Health to the given units.
    /// </summary>
    /// <param name="source">The unit giving the Health.</param>
    /// <param name="recipients">The list of receiving units.</param>
    /// <param name="amount">The amount of Health for each unit to regenerate.</param>
    public void RegenerateHealth(Unit source, List<Unit> recipients, float amount)
    {
        foreach (Unit recipient in recipients)
        {
            recipient.AddHealth(amount);
        }
    }

    /// <summary>
    /// Removes Health from the given units.
    /// </summary>
    /// <param name="source">The unit removing the Health.</param>
    /// <param name="recipients">The list of receiving units.</param>
    /// <param name="amount">The amount of Health for each unit to lose.</param>
    public void RemoveHealth(Unit source, List<Unit> recipients, float amount)
    {
        foreach (Unit recipient in recipients)
        {
            recipient.AddHealth(amount * -1f);
        }
    }

    #endregion

    #region Turn Meter

    /// <summary>
    /// Regenerates Turn Meter to the given units.
    /// </summary>
    /// <param name="source">The unit giving the Turn Meter.</param>
    /// <param name="recipients">The list of receiving units.</param>
    /// <param name="amount">The amount of Turn Meter for each unit to regenerate.</param>
    public void RegenerateTurnMeter(Unit source, List<Unit> recipients, float amount)
    {
        foreach (Unit recipient in recipients)
        {
            recipient.AddTurnMeter(amount, false);
        }
    }

    /// <summary>
    /// Remmoves Turn Meter from the given units.
    /// </summary>
    /// <param name="source">The unit removing the Turn Meter.</param>
    /// <param name="recipients">The list of receiving units.</param>
    /// <param name="amount">The amount of Turn Meter for each unit to lose.</param>
    public void RemoveTurnMeter(Unit source, List<Unit> recipients, float amount)
    {
        foreach (Unit recipient in recipients)
        {
            recipient.AddTurnMeter(amount * -1f, false);
            if (!recipient.ReadyForTurn)
            {
                RemoveTurnReadyUnit(recipient);
            }
        }
    }

    #endregion

    #region Attack

    /// <summary>
    /// Perform the given attack at the current target tile.
    /// </summary>
    /// <param name="source">The attacking unit.</param>
    /// <param name="attackData">All data related to the attack.</param>
    public void Attack(Unit source, AttackData attackData)
    {
        // Determine list of targets
        List<Tuple<Unit, float>> targetWeights = GridManager.instance.EvaluateAttackPattern(attackData.pattern, targetTileTeam, targetTileRow, targetTileCol);

        // Evaluate attack against each target separately
        foreach (Tuple<Unit, float> targetWeight in targetWeights)
        {
            Unit target = targetWeight.Item1;
            float weight = targetWeight.Item2;
            target.ReceiveAttack(source, attackData, weight);
        }
    }

    /// <summary>
    /// Perform the given attack against all receiving units. Each target receives 100% damage weight. Use this method for attacks that use queries instead of the target tile to determine targets.
    /// </summary>
    /// <param name="source">The attacking unit.</param>
    /// <param name="targets">List of units receiving the attack.</param>
    /// <param name="attackData">All data related to the attack.</param>
    public void Attack(Unit source, List<Unit> targets, AttackData attackData)
    {
        // Evaluate attack against each target separately
        foreach (Unit target in targets)
        {
            target.ReceiveAttack(source, attackData, 1f);
        }
    }

    #endregion

    #region Status Effects

    /// <summary>
    /// Applies the given Status Effects to all recipient units.
    /// </summary>
    /// <param name="source">The unit giving the Status Effect.</param>
    /// <param name="recipients">The list of receiving units.</param>
    /// <param name="effects">The list of Status Effect appliers to attempt on each recipient.</param>
    public void AddStatusEffects(Unit source, List<Unit> recipients, List<StatusEffectApplier> effects)
    {
        foreach (Unit recipient in recipients)
        {
            foreach (StatusEffectApplier effectApplier in effects)
            {
                recipient.ReceiveStatusEffect(source, effectApplier);
            }
        }
    }

    #endregion
}
