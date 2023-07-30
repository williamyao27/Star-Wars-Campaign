using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents an instance of a unit and handles all data and logic related to that instance. Most of the instance's attributes will be derived from the base data shared by all units of its type. The remaining attributes represent instance-specific information related to the unit's activities in the current battle context.
/// </summary>
public class Unit : MonoBehaviour
{
    [SerializeField] private UnitDisplay display;
    public UnitData BaseData { get; set; }
    public int TeamNumber { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public float Health { get; set; }
    public float Armor { get; set; }
    public float TurnMeter { get; set; }  // In percentage points, i.e. unit takes turn at TurnMeter == 100f
    public List<ActiveAbility> ActiveAbilities { get; set; }
    public List<PassiveAbility> PassiveAbilities { get; set; }
    public List<Modifier> Modifiers { get; set; }

    // Get current stats from base data plus all modifiers
    public Stats CurrentStats
    {
        get
        {
            return Stats.ApplyModifiers(BaseData.stats, Modifiers);
        }
    }
    
    // Get this unit's Basic Ability; by definition it is their first active Ability
    public ActiveAbility BasicAbility
    {
        get
        {
            return ActiveAbilities[0];
        }
    }

    // Get this unit's list of allied units based on its team number
    public List<Unit> Allies
    {
        get
        {
            return (TeamNumber == 0) ? GameManager.instance.Team0 : GameManager.instance.Team1;
        }
    }

    // Get this unit's list of enemy units based on its team number
    public List<Unit> Enemies
    {
        get
        {
            return (TeamNumber == 0) ? GameManager.instance.Team1 : GameManager.instance.Team0;
        }
    }

    /// <summary>
    /// Initializes all data for the unit and updates its display accordingly.
    /// </summary>
    public void Initialize()
    {
        // Stats
        Health = CurrentStats.maxHealth;
        Armor = CurrentStats.maxArmor;
        TurnMeter = 0f;
        
        // Active Abilities
        ActiveAbilities = new List<ActiveAbility>();
        foreach (ActiveAbilityData data in BaseData.activeAbilities)
        {
            ActiveAbilities.Add(new ActiveAbility(data));
        }
        
        // Passive Abilities
        PassiveAbilities = new List<PassiveAbility>();
        foreach (PassiveAbilityData data in BaseData.passiveAbilities)
        {
            PassiveAbilities.Add(new PassiveAbility(data));
        }

        // Connect unit to its tile
        GridManager.instance.ConnectUnitToTile(this);

        // Visual
        display.UpdateHealthArmorBar(Health, CurrentStats.maxHealth, Armor, CurrentStats.maxArmor);
        display.UpdateTurnMeterBar(TurnMeter);
    }

    #region Turn Meter

    /// <summary>
    /// Updates the unit's Turn Meter by the given amount and clamps between 0 and 1.
    /// </summary>
    /// <param name="amount">The amount by which to increase the unit's TM. Can be negative (i.e. TM loss).</param>
    /// <returns>Whether the unit has reached 100% TM.</returns>
    private bool AddTurnMeter(float amount)
    {
        TurnMeter += amount;
        TurnMeter = Mathf.Clamp(TurnMeter, 0f, 100f);
        display.UpdateTurnMeterBar(TurnMeter); // Visual
        return TurnMeter == 100f;
    }
    
    /// <summary>
    /// Updates the unit's Turn Meter based on their Speed (natural TM generation). TM is generated in "frames", i.e. 1% of Speed in percentage points at a time.
    /// </summary>
    /// <returns>Whether the unit has reached 100% TM.</returns>
    public bool GenerateTurnMeterFromSpeed()
    {
        float amount = CurrentStats.speed * 0.01f;
        return AddTurnMeter(amount);
    }
    
    #endregion

    #region Health

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    private void AddHealth(float amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health, 0f, CurrentStats.maxHealth);
        display.UpdateHealthArmorBar(Health, CurrentStats.maxHealth, Armor, CurrentStats.maxArmor);  // Visual
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    private void AddArmor(float amount)
    {
        Armor += amount;
        Armor = Mathf.Clamp(Armor, 0f, CurrentStats.maxArmor);
        display.UpdateHealthArmorBar(Health, CurrentStats.maxHealth, Armor, CurrentStats.maxArmor);  // Visual
    }

    #endregion

    #region Turn

    public void BeginTurn()
    {
        // Display Abilities to player
        AbilityPaletteManager.instance.ShowAbilities(ActiveAbilities);
    }

    public void EndTurn()
    {
        TurnMeter = 0f;  // Reset Turn Meter
    }

    #endregion

    #region Attacks & Damage

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attack"></param>
    /// <param name="weight"></param>
    public void ReceiveAttack(AttackData attackData, float weight)
    {
        // Terrain check; if the attack cannot strike the unit's terrain, ignore it completely
        if (!IsTargetableTerrain(this, attackData))
        {
            return;
        }

        // Evasion check
        int chanceToHit = attackData.accuracy - CurrentStats.evasion;
        if (Random.Range(0, 100) < chanceToHit)  // Attack hits
        {
            float rawDamage = attackData.damage * weight;

            // Critical Hit check
            int chanceToCrit = attackData.critChance - CurrentStats.critAvoidance;
            if (Random.Range(0, 100) < chanceToCrit)  // Attack crits
            {
                rawDamage *= attackData.critDamage;
            }

            ReceiveDamage(rawDamage, attackData.damageType, attackData.armorPen);
        }
        else  // Attack is Evaded
        {
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rawAmount"></param>
    /// <param name="type"></param>
    /// <param name="armorPen"></param>
    private void ReceiveDamage(float rawAmount, DamageType type, float armorPen)
    {
        // Compute reduction from Defense
        float selectedDefense = (type == DamageType.Physical) ? CurrentStats.physicalDefense : CurrentStats.specialDefense;
        float amount = rawAmount * (1f - selectedDefense / 100f);
        
        // Determine what proportion of damage should go to Armor and Health
        float amountToArmor = Mathf.Min(amount * (1f - armorPen), Armor);
        float amountToHealth = amount - amountToArmor;
        AddHealth(amountToHealth * -1f);
        AddArmor(amountToArmor * -1f);

        Debug.Log($"{BaseData.name} received {amount} Damage (Health: {amountToHealth}, Armor: {amountToArmor}, raw: {rawAmount}). Remaining HP: {Health + Armor}.");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attackData"></param>
    /// <returns></returns>
    public static bool IsTargetableTerrain(Unit target, AttackData attackData)
    {
        return attackData.targetableTerrains.Contains(target.BaseData.terrain);
    }

    #endregion

}
