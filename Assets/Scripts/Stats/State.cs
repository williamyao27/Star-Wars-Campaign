using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores the state of a unit that defines what behaviors it may perform. By default, all attributes are false.
/// </summary>
public class State
{
    public bool skipTurn;
    public bool isTaunting;

    /// <summary>
    /// Apply the list of Status Effects to this state. If a particular attribute is set to true by any Status Effect, it will remain true regardless of all other Status Effects.
    /// </summary>
    /// <param name="statusEffects">The list of Status Effects to apply.</param>
    /// <returns>The current state based on modifications from the Status Effects.</returns>
    public static State ApplyStatusEffects(List<StatusEffect> statusEffects)
    {
        State state = new State();

        foreach (StatusEffect effect in statusEffects)
        {
            if (effect.Data.stateModifier != null)
            {
                state.skipTurn |= effect.Data.stateModifier.skipTurn;
                state.isTaunting |= effect.Data.stateModifier.isTaunting;
            }
        }

        return state;
    }
}