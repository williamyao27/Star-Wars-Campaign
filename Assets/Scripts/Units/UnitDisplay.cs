using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class contains all the logic for updating the visual representation associated with an instance of a unit.
/// </summary>
public class UnitDisplay : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private Image armorBar;
    [SerializeField] private Image turnMeterBar;

    /// <summary>
    /// Updates the unit's Health and Armor bar visual based on their proportion of remaining Health and Armor
    /// </summary>
    /// <param name="health">Unit's current Health.</param>
    /// <param name="maxHealth">Unit's Max Health.</param>
    /// /// <param name="armor">Unit's current Armor.</param>
    /// <param name="maxArmor">Unit's Max Armor.</param>
    public void UpdateHealthArmorBar(float health, float maxHealth, float armor, float maxArmor)
    {   
        // Determine relative width of bars based on max values; all widths are normalized as the total width should be 1
        float healthWidth = maxHealth / (maxHealth + maxArmor);
        float armorWidth = 1f - healthWidth;
        healthBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthWidth);
        armorBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, armorWidth);

        // Determine placement of Armor bar; should start where the filled portion of the Health bar ends
        float healthOffset = health / (maxHealth + maxArmor);
        Vector2 armorBarPosition = armorBar.transform.localPosition;
        armorBarPosition.x = -0.5f + healthOffset;
        armorBar.transform.localPosition = armorBarPosition;

        // Determine proportion of each bar to fill in
        float healthPercent = health / maxHealth;
        float armorPercent = (maxArmor > 0) ? armor / maxArmor : 0;
        healthBar.fillAmount = healthPercent;
        armorBar.fillAmount = armorPercent;
    }

    /// <summary>
    /// Updates the unit's Turn Meter bar visual based on their Turn Meter.
    /// </summary>
    /// <param name="turnMeter">Unit's current Turn Meter.</param>
    public void UpdateTurnMeterBar(float turnMeter)
    {   
        turnMeterBar.fillAmount = turnMeter / 100f;  // Divide by 100 as Turn Meter is in percentage points, not decimal
    }
}
