using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a singleton manager that generates and tracks input on the battlefield grid.
/// </summary>
public class GridManager : Singleton<GridManager>
{
    [SerializeField] private int width, height;  // Dimensions of each team's half of the grid
    [SerializeField] private Tile tilePrefab;
    private Tile[,,] grid;  // 3D array to store all tiles; first dimension is the team to which the tile belongs, followed by row and column
    
    private void Start()
    {
        GenerateGrid();
    }

    /// <summary>
    /// Generates the battle grid out of Tile objects
    /// </summary>
    private void GenerateGrid()
    {
        // Initialize grid array
        grid = new Tile[2, height, width];

        for (int teamNumber = 0; teamNumber < 2; teamNumber++)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    // Compute tile position
                    float tileX = ComputeTileX(col);
                    float tileY = ComputeTileY(row, teamNumber);
                    
                    // Instantiate tile
                    Tile tile = Instantiate(tilePrefab, new Vector3(tileX, tileY, 0), Quaternion.identity, transform);
                    tile.Initialize(teamNumber, row, col);

                    // Track tile in grid array
                    grid[teamNumber, row, col] = tile;
                }
            }
        }
    }

    /// <summary>
    /// Computes the x-position of a tile in a given column.
    /// </summary>
    /// <param name="col">The column index of the tile.</param>
    /// <returns>The x-position of the tile.</returns>
    private float ComputeTileX(int col)
    {
        float tileWidth = tilePrefab.transform.localScale.x;
        return col * tileWidth;
    }

    /// <summary>
    /// Computes the y-position of a tile in a given row.
    /// </summary>
    /// <param name="row">The row index of the tile.</param>
    /// <param name="teamNumber">The team whose half of the grid the tile belongs to.</param>
    /// <returns>The y-position of the tile.</returns>
    private float ComputeTileY(int row, int teamNumber)
    {
        float tileHeight = tilePrefab.transform.localScale.y;
        float frontier = (width - 1f) * tileHeight;  // Frontier is the y-level where the top row of team 0's grid is anchored; needs to fit height - 1 rows beneath it
        if (teamNumber == 0)
        {
            // Team 0: grid is formed from the frontier line with the y-level moving down towards the rear
            return frontier - row * tileHeight;
        }
        else
        {
            // Team 1: grid is formed from the frontier line with the y-level moving up towards the rear. It starts at 1.5 widths past the frontier line, where 1 comes from the width of the last row on team 1's grid, and 0.5 comes from the gap
            return frontier + 1.5f * tileHeight + row * tileHeight;
        }
    }

    /// <summary>
    /// Attaches the given unit instance to the corresponding Tile script and nests its GO accordingly.
    /// </summary>
    /// <param name="unit">The given unit.</param>
    public void ConnectUnitToTile(Unit unit)
    {
        // Retrieve corresponding tile
        Tile tile = grid[unit.TeamNumber, unit.Row, unit.Col];

        // Associate unit instance to tile
        tile.Unit = unit;

        // Nest unit object under tile object
        unit.transform.SetParent(tile.transform);
        unit.transform.localPosition = new Vector2(0, 0);
    }
    
    /// <summary>
    /// Generates a list of all units in the given area of the grid.
    /// </summary>
    /// <param name="teamNumber">Which team's grid to scan.</param>
    /// <param name="rowStart">The starting row.</param>
    /// <param name="colStart">The starting column.</param>
    /// <param name="rowEnd">The ending row.</param>
    /// <param name="colEnd">The ending column.</param>
    /// <returns></returns>
    public List<Unit> GetUnitsInArea(int teamNumber, int rowStart, int rowEnd, int colStart, int colEnd)
    {
        List<Unit> unitsInArea = new List<Unit>();

        // Note that row and column boundaries are inclusive
        for (int row = rowStart; row <= rowEnd; row++)
        {
            for (int col = colStart; col <= colEnd; col++)
            {
                Unit unit = grid[teamNumber, row, col].Unit;
                if (unit != null)
                {
                    unitsInArea.Add(unit);
                }
            }
        }
        return unitsInArea;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    // public List<Unit> GetAdjacentUnits(int row, int col)
    // {
    // }

    /// <summary>
    /// Highlight all tiles on the grid that can serve as the target tile for the given attack from the current user.
    /// </summary>
    /// <param name="attacker">The unit for which the target tile is being selected.</param>
    /// <param name="attackData">Contains detail on how the target tile can be selected.</param>
    public void SetTargetableTiles(Unit attacker, AttackData attackData)
    {
        // Call each tile to set their own targetability based on the provided attack
        for (int teamNumber = 0; teamNumber < 2; teamNumber++)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    grid[teamNumber, row, col].SetTargetability(attacker, attackData);
                }
            }
        }
    }

    /// <summary>
    /// Remove all targetability highlights and terrain warnings from the grid. 
    /// </summary>
    public void HideTargetableTiles()
    {
        for (int teamNumber = 0; teamNumber < 2; teamNumber++)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    grid[teamNumber, row, col].HideTargetability();
                }
            }
        }
    }
    
    /// <summary>
    /// Performs the given operation on tiles in the grid based on the given attack pattern.
    /// </summary>
    /// <param name="attackPattern">Attack pattern as a 2D array of weights.</param>
    /// <param name="teamNumber">The team whose half of the grid the target tile belongs to.</param>
    /// <param name="patternAnchorRow">The row index of the target tile.</param>
    /// <param name="patternAnchorCol">The column index of the target tile.</param>
    /// <param name="processTileAction">The action to perform with each tile and corresponding weight in the pattern.</param>
    private void ProcessAttackPattern(float[,] attackPattern, int[] patternAnchor, int teamNumber, int patternAnchorRow, int patternAnchorCol, Action<Tile, float> processTileAction)
    {
        // Retrieve pattern dimensions
        int patternHeight = attackPattern.GetLength(0);
        int patternWidth = attackPattern.GetLength(1);

        // Traverse attack pattern
        for (int row = 0; row < patternHeight; row++)
        {
            for (int col = 0; col < patternWidth; col++)
            {
                // Compute which board tile the current attack pattern tile corresponds to
                int projectedCol = patternAnchorCol + col - patternAnchor[0];
                int projectedRow = patternAnchorRow + row - patternAnchor[1];

                // Get damage weight on the current tile
                float weight = attackPattern[row, col];

                // If the projected tile coordinates are valid for the board, and the damage weight is non-zero, process the given tile action
                if (0 <= projectedCol && projectedCol < width && 0 <= projectedRow && projectedRow < height && weight > 0)
                {
                    Tile projectedTile = grid[teamNumber, projectedRow, projectedCol];
                    processTileAction(projectedTile, weight);
                }
            }
        }
    }

    /// <summary>
    /// For the given attack pattern, determines the list of target units and the corresponding weight of the attack against them based on the placement of units on the board and the given pattern center.
    /// </summary>
    /// <param name="attackPattern">Attack pattern as a 2D array of weights.</param>
    /// <param name="teamNumber">The team whose half of the grid the target tile belongs to.</param>
    /// <param name="patternAnchorRow">The row index of the target tile.</param>
    /// <param name="patternAnchorCol">The column index of the target tile.</param>
    /// <returns>List of tuples where each tuple is a target unit and its corresponding damage weight.</returns>
    public List<Tuple<Unit, float>> EvaluateAttackPattern(float[,] attackPattern, int[] patternAnchor, int teamNumber, int patternAnchorRow, int patternAnchorCol)
    {
        List<Tuple<Unit, float>> targetWeights = new List<Tuple<Unit, float>>();

        ProcessAttackPattern(attackPattern, patternAnchor, teamNumber, patternAnchorRow, patternAnchorCol, (tile, weight) =>
            {
                Unit projectedUnit = tile.Unit;

                // If a unit exists on the projected tile, add it and its corresponding weight to the list
                if (projectedUnit != null)
                {
                    targetWeights.Add(new Tuple<Unit, float>(projectedUnit, weight));
                }
            }
        );

        return targetWeights;
    }

    /// <summary>
    /// For the given attack pattern, highlights all tiles that lie within the pattern based on the given pattern center.
    /// </summary>
    /// <param name="attackPattern">Attack pattern as a 2D array of weights.</param>
    /// <param name="teamNumber">The team whose half of the grid the target tile belongs to.</param>
    /// <param name="patternAnchorRow">The row index of the target tile.</param>
    /// <param name="patternAnchorCol">The column index of the target tile.</param>
    public void VisualizeAttackPattern(AttackData attackData, int teamNumber, int patternAnchorRow, int patternAnchorCol)
    {
        ProcessAttackPattern(attackData.pattern, attackData.patternAnchor, teamNumber, patternAnchorRow, patternAnchorCol, (tile, weight) =>
        {
            tile.SetWeightHighlight(weight);
        });
    }

    /// <summary>
    /// Remove all damage weight highlights and terrain warnings from the grid.
    /// </summary>
    public void HideVisualizedAttackPattern()
    {
        for (int teamNumber = 0; teamNumber < 2; teamNumber++)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    grid[teamNumber, row, col].SetWeightHighlight(0f);
                }
            }
        }
    }
}
