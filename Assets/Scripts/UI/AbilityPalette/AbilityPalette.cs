using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a singleton manager that manages the Ability Palette UI.
/// </summary>
public class AbilityPaletteManager : Singleton<AbilityPaletteManager>
{
    [SerializeField] private AbilityButton abilityButtonPrefab;

    /// <summary>
    /// Displays the Abilities available to use to the unit whose turn it is as buttons on the ability palette.
    /// </summary>
    /// <param name="unit"></param>
    public void ShowAbilities(List<ActiveAbility> abilities)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            ActiveAbility currentAbility = abilities[i];

            // Instantiate Ability button
            AbilityButton abilityButtonGO = Instantiate(abilityButtonPrefab, new Vector3(i + 5, 2, 0), Quaternion.identity);
            abilityButtonGO.transform.parent = transform;
            abilityButtonGO.Initialize(currentAbility);
        }
    }

    /// <summary>
    /// Removes all currently displayed Ability buttons.
    /// </summary>
    public void HideAbilities()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
