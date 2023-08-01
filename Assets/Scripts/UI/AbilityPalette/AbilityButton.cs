using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class 
/// </summary>
public class AbilityButton : HoverHighlightable
{
    public ActiveAbility Ability {get; set; }

    /// <summary>
    /// Initializes the Ability button's name and associated Ability.
    /// </summary>
    /// <param name="ability">The associated Ability.</param>
    public void Initialize(ActiveAbility ability)
    {
        Ability = ability;
        name = Ability.Data.name;
    }
    
    private void OnMouseDown()
    {
        // Choose for the unit whose turn it currently is to use the Ability associated with this button. By assumption, the button is only displayed if the unit is taking a turn, so no explicit game state check is required.
        GameManager.instance.SelectAbility(Ability);
    }
}
