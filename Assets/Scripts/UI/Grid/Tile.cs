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
    [SerializeField] private GameObject weightHighlight;
    [SerializeField] private GameObject terrainWarning;
    [SerializeField] private Color baseColor, darkColor;
    [SerializeField] private new SpriteRenderer renderer;
    public Unit Unit { get; set; }  // Associated unit instance
    public int TeamNumber { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    private bool isTargetable;
    
    /// <summary>
    /// Initializes the tile object's name and color based on its position on the board.
    /// </summary>
    /// <param name="teamNumber">On which team's half of the board this tile belongs.</param>
    /// <param name="row">The row of this tile on its half of the board.</param>
    /// <param name="col">The column of this tile on its half of the board.</param>
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
    /// Check whether this tile is targetable based on the location of the attacker and the attack specifications and highlight the tile if so.
    /// </summary>
    /// <param name="attacker">The attacking unit.</param>
    /// <param name="attackData">Contains detail on how the target tile can be selected</param>
    public void SetTargetability(Unit attacker, AttackData attackData)
    {
        isTargetable = true;

        // Invalid if tile is on the attacker's side of the grid
        if (TeamNumber == attacker.TeamNumber) {
            isTargetable = false;
        }

        // Invalid if column is out of range
        int distance = (attackData.lineOfFireModifiers.Contains(LineOfFireModifier.Rear)) ? 3 - Col + 1 : attacker.Col + Col + 1;
        if (distance > attackData.range)
        {
            isTargetable = false;
        }

        // Invalid if the LoF is Fixed and the tile is in a different row
        if (attackData.lineOfFireModifiers.Contains(LineOfFireModifier.Fixed) && attacker.Row != Row)
        {
            isTargetable = false;
        }

        // Invalid if blocking units exist in the same row in front
        List<Unit> unitsInFront = new List<Unit>();
        if (attackData.lineOfFireModifiers.Contains(LineOfFireModifier.Rear) && Col <= 2)
        {
            unitsInFront = GridManager.instance.GetUnitsInArea(TeamNumber, Row, Row, Col + 1, 3);   
        }
        else if (!attackData.lineOfFireModifiers.Contains(LineOfFireModifier.Rear) && Col >= 1)
        {
            unitsInFront = GridManager.instance.GetUnitsInArea(TeamNumber, Row, Row, 0, Col - 1);
        }
        foreach (Unit unit in unitsInFront) {
            // If LoF == Contact, blocked by everything; if LOF == Direct, blocked by Covering units
            if (attackData.lineOfFire == LineOfFire.Contact || (attackData.lineOfFire == LineOfFire.Direct && !(unit.Data.cover)))
            {
                isTargetable = false;
                break;
            }
        }

        // Invalid if the unit on this tile is not Taunting and another unit in targetable range is
        if (Unit == null || !Unit.CurrentState.isTaunting)
        {
            List<Unit> targetableUnits = new List<Unit>();

            // Check only this row if the attack is fixed, otherwise check all rows 0-4
            int bottomRow = (attackData.lineOfFireModifiers.Contains(LineOfFireModifier.Fixed)) ? Row : 0;
            int topRow = (attackData.lineOfFireModifiers.Contains(LineOfFireModifier.Fixed)) ? Row : 4;

            if (attackData.lineOfFireModifiers.Contains(LineOfFireModifier.Rear))
            {
                targetableUnits = GridManager.instance.GetUnitsInArea(TeamNumber, bottomRow, topRow, 3 - attackData.range + 1, 3);   
            }
            else if (!attackData.lineOfFireModifiers.Contains(LineOfFireModifier.Rear))
            {
                targetableUnits = GridManager.instance.GetUnitsInArea(TeamNumber, bottomRow, topRow, 0, attackData.range - Col - attacker.Col - 1);
            }
            foreach (Unit unit in targetableUnits) {
                // If any other unit in targetableUnits is Taunting, this tile cannot be targeted
                if (Unit != unit && unit.CurrentState.isTaunting)
                {
                    isTargetable = false;
                    break;
                }
            }
        }

        // Show targetability
        targetabilityHighlight.SetActive(isTargetable);

        // Additionally, show terrain warning if the unit on this tile cannot be hit by the attack
        if (Unit != null && !Unit.IsTargetableTerrain(attackData))
        {
            terrainWarning.SetActive(true);
        }
    }

    /// <summary>
    /// Clear the tile's state of targetability and hide any related indicators on the tile.
    /// </summary>
    public void HideTargetability()
    {
        isTargetable = false;
        targetabilityHighlight.SetActive(isTargetable);
        crosshair.SetActive(false);
        terrainWarning.SetActive(false);
    }

    /// <summary>
    /// Sets the transparency damage weight highlight of the tile. If the given weight is positive, also makes the targetability highlight for this tile completely transparent to prevent color clashing. If not, reverts it to full opacity.
    /// </summary>
    /// <param name="weight">The damage weight.</param>
    public void SetWeightHighlight(float weight)
    {
        // Set red weight highlight
        SpriteRenderer weightHighlightRenderer = weightHighlight.GetComponent<SpriteRenderer>();
        Color highlightColor = weightHighlightRenderer.color;
        highlightColor.a = weight;
        weightHighlightRenderer.color = highlightColor;

        // Toggle the transparency of the targetability highlight based on the weight
        SpriteRenderer targetabilityHighlightRenderer = targetabilityHighlight.GetComponent<SpriteRenderer>();
        highlightColor = targetabilityHighlightRenderer.color;
        highlightColor.a = (weight > 0f) ? 0f : 0.2f;  // Prefab opacity value is 51/255 = 0.2
        targetabilityHighlightRenderer.color = highlightColor;
    }

    private void OnMouseEnter()
    {
        // If a target tile input is currently required and this is an eligible target, display a crosshair over the tile as well as a projection of the attack pattern
        if (GameManager.instance.CurrentRequiredInput == InputType.TargetEnemyTile && isTargetable)
        {
            crosshair.SetActive(true);
            GridManager.instance.VisualizeAttackPattern(GameManager.instance.CurrentSelectedAbility.Data.attackData, TeamNumber, Row, Col);
        }
    }

    private void OnMouseExit()
    {
        // Reset any visualization of the currently selected attack's targets
        crosshair.SetActive(false);
        GridManager.instance.HideVisualizedAttackPattern();
    }

    private void OnMouseDown()
    {
        // If a target tile input is currently required and this is an eligible target, choose this target tile
        if (GameManager.instance.CurrentRequiredInput == InputType.TargetEnemyTile && isTargetable)
        {
            GameManager.instance.SelectTargetTile(TeamNumber, Row, Col);
        }
    }
}
