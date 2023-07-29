using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class 
/// </summary>
public class AbilityButton : MonoBehaviour
{
    public ActiveAbility Ability {get; set; }
    [SerializeField] private GameObject highlight;

    public void Initialize(ActiveAbility ability)
    {
        Ability = ability;
        name = Ability.BaseData.name;
    }

    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }
    
    private void OnMouseDown()
    {
        // Choose for the unit whose turn it currently is to use the Ability associated with this button
        GameManager.instance.SelectAbility(Ability);
    }
}
