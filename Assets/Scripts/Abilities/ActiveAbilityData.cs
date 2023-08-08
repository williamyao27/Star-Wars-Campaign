using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This struct stores all information related to an Active Ability.
/// </summary>
[Serializable]
public struct ActiveAbilityData
{
    public string name;
    public string description;
    public string picture;

    public int maxCooldown;
    public bool startOnCooldown;
    public InputType? requiredInput;  // Each Ability requires only one step of input from the player at most
    public Action[] actions;
}

public enum InputType
{
    TargetEnemyTile,
    Ally
}
