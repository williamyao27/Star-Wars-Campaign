using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class represents a single distinct action that may be performed as part of an Ability. It is highly dependent on contextual data, specifically information on which Ability the Action is part of, and which unit is using that Ability.
/// </summary>
[Serializable]
public class Action
{
    public string type;  // Determines what type of Action this is; TODO: split class into child classes and use inheritance.

    // Attack attributes

    public void Execute(ActiveAbility ability)  // TODO: Accept passives as well
    {
        switch (type)
        {
            case "attack":
                GameManager.instance.Attack(ability.BaseData.attackData);
                break;
            default:
                break;
        }
    }
}