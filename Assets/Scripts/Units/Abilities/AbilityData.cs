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

    public InputType? input;
    public Action[] actions;
    public int maxCooldown = 0;
    public bool startOnCooldown = false;
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
}
