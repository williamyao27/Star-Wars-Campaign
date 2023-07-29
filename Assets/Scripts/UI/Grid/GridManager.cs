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
                    float tileX = ComputeTileX(col, teamNumber);
                    float tileY = ComputeTileY(row);
                    
                    // Instantiate tile
                    Tile tile = Instantiate(tilePrefab, new Vector2(tileX, tileY), Quaternion.identity);
                    tile.transform.parent = transform;
                    tile.Initialize(teamNumber, col, row);

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
    /// <param name="teamNumber">The team whose half of the grid the tile belongs to.</param>
    /// <returns>The x-position of the tile.</returns>
    private float ComputeTileX(int col, int teamNumber)
    {
        float tileWidth = tilePrefab.transform.localScale.x;
        float frontier = ((float)width - 1f) * tileWidth;  // Frontier is the x-level where the left column of team 1's grid is anchored; needs to fit width - 1 columns on the left
        if (teamNumber == 0)
        {
            // Team 0: grid is formed from the frontier line with the x-level moving left towards the rear
            return frontier - (float)col * tileWidth;
        }
        else
        {
            // Team 1: grid is formed from the frontier line with the x-level moving right towards the rear
            return frontier + 1.5f * tileWidth + (float)col * tileWidth;
        }
    }

    /// <summary>
    /// Computes the y-position of a tile in a given row.
    /// </summary>
    /// <param name="row">The row index of the tile.</param>
    /// <returns>The y-position of the tile.</returns>
    private float ComputeTileY(int row)
    {
        float tileHeight = tilePrefab.transform.localScale.y;
        return (float)row * tileHeight;
    }

    /// <summary>
    /// Attaches the given unit instance to the corresponding tile script and nests its GO accordingly.
    /// </summary>
    /// <param name="unit">The given unit.</param>
    /// <param name="teamNumber">The team which the unit belongs to.</param>
    /// <param name="row">The row position of the unit.</param>
    /// <param name="col">The column position of the unit.</param>
    public void ConnectUnitToTile(Unit unit, int teamNumber, int row, int col)
    {
        // Retrieve corresponding tile
        Tile tile = grid[teamNumber, row, col];

        // Associate unit instance to tile
        tile.Unit = unit;

        // Nest unit object under tile object
        unit.transform.SetParent(tile.transform);
        unit.transform.localPosition = new Vector2(0, 0);
    }
    
    /// <summary>
    /// For the given attack pattern, determines the list of target units and the corresponding weight of the attack against them based on the placement of units on the board and the given target tile.
    /// </summary>
    /// <param name="attackPattern">Attack pattern as float weights in a 2D array.</param>
    /// <param name="teamNumber">The team whose half of the grid the target tile belongs to.</param>
    /// <param name="patternCenterRow">The row index of the tile.</param>
    /// <param name="patternCenterCol">The column index of the tile.</param>
    /// <returns>List of tuples where each tuple is a target unit and its corresponding attack weight.</returns>
    public List<Tuple<Unit, float>> EvaluateAttackPattern(float[,] attackPattern, int teamNumber, int patternCenterRow, int patternCenterCol)
    {
        List<Tuple<Unit, float>> targetWeights = new List<Tuple<Unit, float>>();

        // Retrieve pattern dimensions
        int patternWidth = attackPattern.GetLength(0);
        int patternHeight = attackPattern.GetLength(1);

        // Traverse attack pattern
        for (int row = 0; row < patternHeight; row++)
        {
            for (int col = 0; col < patternWidth; col++)
            {   
                // Compute which board tile the current attack pattern tile corresponds to
                int projectedCol = patternCenterCol + col - patternWidth / 2;
                int projectedRow = patternCenterRow + row - patternHeight / 2;

                // Get attack weight on the current tile as defined in attackPattern
                float attackWeight = attackPattern[row, col];

                // If the projected tile coordinates are valid for the board, and the attack weight is non-zero...
                if (0 <= projectedCol && projectedCol < width && 0 <= projectedRow && projectedRow < height && attackWeight > 0)
                {
                    Unit projectedUnit = grid[teamNumber, projectedRow, projectedCol].Unit;

                    // And if a unit exists on the projected tile...
                    if (projectedUnit != null)
                    {
                        // Add to the list of targets
                        Tuple<Unit, float> targetWeight = new Tuple<Unit, float>(projectedUnit, attackWeight);
                        targetWeights.Add(targetWeight);
                    }
                }
            }
        }

        return targetWeights;
    }
}