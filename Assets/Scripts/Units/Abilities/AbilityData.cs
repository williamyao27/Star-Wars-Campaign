using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[Serializable]
public class ActiveAbilityData
{
    public string name;
    public string description;
    public string picture;

    public Action[] actions;
    public int maxCooldown;
    public bool startOnCooldown;
    
    // Input specifications
    public InputType? requiredInput;
    public TargetTileSelector targetTileSelector;  // Used for attacks
}

/// <summary>
/// 
/// </summary>
[Serializable]
public class PassiveAbilityData
{
    public string name;
    public string description;
    public string picture;
}

public enum InputType
{
    TargetTile,
    TargetAlly
}
