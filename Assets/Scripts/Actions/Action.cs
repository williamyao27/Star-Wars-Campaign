using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class represents a single distinct action that may be performed as part of an Ability. It is highly dependent on contextual data, specifically information on which Ability the Action is part of, and which unit is using that Ability.
/// TODO: Split this class into child classes and use inheritance.
/// </summary>
[Serializable]
public class Action
{
    public string type;
    public int chance;

    // Perform Attack

    // Grant Buffs
    public List<string> buffNames = new List<string>();

    public void Execute(Unit user, ActiveAbility ability)  // TODO: Accept passives as well
    {
        switch (type)
        {
            case "PerformAttack":
                GameManager.instance.Attack(ability.Data.attackData);
                break;
            case "GrantBuffs":
                List<Unit> recipients = new List<Unit>{user};
                GameManager.instance.GrantBuffs(recipients, buffNames);
                break;
            default:
                break;
        }
    }
}