using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
[Serializable]
public class Action
{
    public string type;  // Determines what type of Action this is; TODO: split class into child classes and use inheritance.

    public AttackEffects attackEffects;

    public void Execute()
    {
        switch (type)
        {
            case "attack":
                GameManager.instance.Attack(attackEffects);
                break;
            default:
                break;
        }
    }
}