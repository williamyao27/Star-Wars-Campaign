using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[Serializable]
public struct ActiveAbilityData
{
    public string name;
    public string description;
    public string picture;

    public int maxCooldown;
    public bool startOnCooldown;
    public InputType? requiredInput;  // Design principle: each Ability should only require one step of input from the player at most.
    public AttackData attackData;  // Instead of storing this with the attack-related Actions, we store it at the Ability-level so that it is easier to identify for target tile identification purposes, and because each Ability is associated with at most one attack.
    public Action[] actions;
}

/// <summary>
/// 
/// </summary>
[Serializable]
public struct PassiveAbilityData
{
    public string name;
    public string description;
    public string picture;
}

public enum InputType
{
    TargetEnemyTile,
    Ally
}
