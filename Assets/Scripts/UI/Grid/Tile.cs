using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a single square on the battlefield grid.
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, darkColor;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;  
    public Unit Unit { get; set; }  // Associated unit instance
    public int TeamNumber { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    
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

    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }
    
    private void OnMouseDown()
    {
        // Choose this tile as the target tile for the current Ability
        GameManager.instance.SelectTargetTile(TeamNumber, Row, Col);
    }
}
