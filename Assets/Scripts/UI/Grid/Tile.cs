using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a single square on the battlefield grid.
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject targetabilityHighlight;
    [SerializeField] private Color baseColor, darkColor;
    [SerializeField] private SpriteRenderer renderer;
    public Unit Unit { get; set; }  // Associated unit instance
    public int TeamNumber { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    private bool isTargetable;
    
    /// <summary>
    /// Initializes the tile object's name and color based on its position on the board.
    /// </summary>
    /// <param name="teamNumber">On which team's half of the board this tile belongs.</param>
    /// <param name="row">The row of this tile relative to its half of the board.</param>
    /// <param name="col">The column of this tile relative to its half of the board.</param>
    public void Initialize(int teamNumber, int row, int col)
    {
        name = $"Team {teamNumber} Tile ({col}, {row})";
        TeamNumber = teamNumber;
        Row = row;
        Col = col;

        // Set alternating color
        bool isDark = (col % 2 == row % 2);  // Dark if x and y are either both even or both odd
        renderer.color = isDark ? darkColor : baseColor;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targeter"></param>
    /// <param name="selector"></param>
    public void SetTargetability(Unit targeter, TargetTileSelector targetTileSelector)
    {
        isTargetable = true;

        // Invalid if tile is on the targeter's side of the grid
        if (TeamNumber == targeter.TeamNumber) {
            isTargetable = false;
        }

        // Invalid if column is out of range
        int distance;
        if (targetTileSelector.lineOfFireModifiers.Contains(LineOfFireModifier.Rear))
        {
            distance = 3 - targeter.Col + 3 - Col + 1;
        }
        else
        {
            distance = targeter.Col + Col + 1;
        }

        if (distance > targetTileSelector.range)
        {
            isTargetable = false;
        }

        // Invalid if the selector is Fixed and the tile is in a different row
        if (targetTileSelector.lineOfFireModifiers.Contains(LineOfFireModifier.Fixed) && targeter.Row != Row)
        {
            isTargetable = false;
        }

        // Invalid if certain units exist in the same row in front
        List<Unit> unitsInFront = new List<Unit>();
        if (targetTileSelector.lineOfFireModifiers.Contains(LineOfFireModifier.Rear) && Col <= 2)
        {
            unitsInFront = GridManager.instance.GetUnitsInArea(TeamNumber, Row, Row, Col + 1, 3);   
        }
        if (!targetTileSelector.lineOfFireModifiers.Contains(LineOfFireModifier.Rear) && Col >= 1)
        {
            unitsInFront = GridManager.instance.GetUnitsInArea(TeamNumber, Row, Row, 0, Col - 1);
        }
        foreach (Unit unit in unitsInFront) {
            // If LoF == Contact, blocked by everything; if LOF == Direct, blocked by covering units
            if (targetTileSelector.lineOfFire == LineOfFire.Contact || (targetTileSelector.lineOfFire == LineOfFire.Direct && !(unit.CurrentStats.cover)))
            {
                isTargetable = false;
            }
        }

        // Invalid if a unit has Taunt in the same column

        // Show targetability
        targetabilityHighlight.SetActive(isTargetable);
    }

    /// <summary>
    /// 
    /// </summary>
    public void HideTargetability()
    {
        isTargetable = false;
        targetabilityHighlight.SetActive(isTargetable);
        crosshair.SetActive(false);  // Also hide the crosshair in case the mouse has not yet exited
    }

    // Note that Tiles respond to mouse enter/exits when a target tile must be selected, but this does not involve a highlight and so they do not derive from HoverHighlightable.
    private void OnMouseEnter()
    {
        // If a target tile input is currently required and this is an eligible target, display a crosshair over the tile
        if (GameManager.instance.CurrentRequiredInput == InputType.TargetTile && isTargetable)
        {
            crosshair.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        crosshair.SetActive(false);
    }

    private void OnMouseDown()
    {
        // If a target tile input is currently required and this is an eligible target, choose this target tile
        if (GameManager.instance.CurrentRequiredInput == InputType.TargetTile && isTargetable)
        {
            GameManager.instance.SelectTargetTile(TeamNumber, Row, Col);
        }
    }
}
