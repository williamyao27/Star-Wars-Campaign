using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents an instance of a unit and handles all data and logic related to that instance. Most of the instance's attributes will be derived from the base data shared by all units of its type. The remaining attributes represent instance-specific information related to the unit's activities in the current battle context.
/// </summary>
public class Unit : MonoBehaviour
{
    [SerializeField] private UnitDisplay display;
    public UnitData Data { get; set; }
    public int TeamNumber { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    private float health;
    private float armor;
    private float turnMeter;  // In percentage points, i.e. unit takes turn at turnMeter == 100f
    private List<ActiveAbility> activeAbilities = new List<ActiveAbility>();
    private List<PassiveAbility> passiveAbilities = new List<PassiveAbility>();
    private List<StatusEffect> statusEffects = new List<StatusEffect>();
    private List<StatusEffect> statusEffectsBeginTurn;

    // Get current stats from base data plus all modifiers
    public Stats CurrentStats
    {
        get
        {
            return Stats.ApplyStatusEffects(Data.stats, statusEffects);
        }
    }
    
    // Get this unit's Basic Ability; by definition it is their first active Ability
    public ActiveAbility BasicAbility
    {
        get
        {
            return activeAbilities[0];
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
        health = CurrentStats.maxHealth;
        armor = CurrentStats.maxArmor;
        turnMeter = 0f;
        
        // Active Abilities
        foreach (ActiveAbilityData abilityData in Data.activeAbilities)
        {
            activeAbilities.Add(new ActiveAbility(abilityData));
        }
        
        // Passive Abilities
        foreach (PassiveAbilityData abilityData in Data.passiveAbilities)
        {
            passiveAbilities.Add(new PassiveAbility(abilityData));
        }

        // Connect unit to its tile
        GridManager.instance.ConnectUnitToTile(this);

        // Visual
        display.UpdateHealthArmorBar(health, CurrentStats.maxHealth, armor, CurrentStats.maxArmor);
        display.UpdateTurnMeterBar(turnMeter);
    }

    #region Turn Meter

    /// <summary>
    /// Updates the unit's Turn Meter by the given amount and clamps between 0 and 1.
    /// </summary>
    /// <param name="amount">The amount by which to increase the unit's TM. Can be negative (i.e. TM loss).</param>
    /// <returns>Whether the unit has reached 100% TM.</returns>
    private bool AddTurnMeter(float amount)
    {
        turnMeter += amount;
        turnMeter = Mathf.Clamp(turnMeter, 0f, 100f);
        display.UpdateTurnMeterBar(turnMeter); // Visual
        return turnMeter == 100f;
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
    /// Add to this unit's current Health.
    /// </summary>
    /// <param name="amount">Amount of Health to add.</param>
    private void AddHealth(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0f, CurrentStats.maxHealth);
        display.UpdateHealthArmorBar(health, CurrentStats.maxHealth, armor, CurrentStats.maxArmor);  // Visual
    }

    /// <summary>
    /// Add to this unit's current Armor.
    /// </summary>
    /// <param name="amount">Amount of Armor to add.</param>
    private void AddArmor(float amount)
    {
        armor += amount;
        armor = Mathf.Clamp(armor, 0f, CurrentStats.maxArmor);
        display.UpdateHealthArmorBar(health, CurrentStats.maxHealth, armor, CurrentStats.maxArmor);  // Visual
    }

    #endregion

    #region Status Effects

    /// <summary>
    /// Checks whether the given unit should receive the given Status Effect, and adds it if so.
    /// </summary>
    /// <param name="effect">The given Status Effect.</param>
    /// <param name="sourceUnit">The unit </param>
    /// <param name="resistible">Whether the given Status Effect can be Resisted.</param>
    public void ReceiveStatusEffect(Unit source, StatusEffect effect, bool resistible)
    {   
        if (effect.Data.type == StatusEffectType.Buff)
        {

        }
        else
        {
            // Resistance check
            int chanceToInflict = source.CurrentStats.potency - CurrentStats.resistance;
            if (Random.Range(0, 100) < chanceToInflict || !resistible)  // Effect is added
            {
            }
            else  // Effect is Resisted
            {
                return;
            }
        }

        // If the effect is not stackable, search for it in the unit's existing Status Effects
        if (!effect.Data.stackable)
        {
            foreach (StatusEffect existingEffect in statusEffects)
            {
                // The unit already has this Status Effect
                if (existingEffect.Data.name == effect.Data.name)
                {
                    // If the existing Status Effect has a longer duration, do nothing
                    if (existingEffect.Duration > effect.Duration)
                    {
                        return;
                    }
                    else  // Otherwise, we remove the Status Effect for replacement and exit (by assumption, there can only be one instance of this type of Status Effect)
                    {
                        statusEffects.Remove(existingEffect);
                        break;
                    }
                }
            }
        }

        statusEffects.Add(effect);
    }

    #endregion

    #region Turn

    public void BeginTurn()
    {
        // Display Abilities to player
        AbilityPaletteManager.instance.ShowAbilities(activeAbilities);

        // Track Status Effects which are present at the beginning of the turn
        statusEffectsBeginTurn = new List<StatusEffect>(statusEffects);
    }

    public void EndTurn()
    {
        // Reset Turn Meter
        turnMeter = 0f;

        // Decrement duration of all Status Effects which were already present at the beginning of this turn
        foreach (StatusEffect effect in statusEffectsBeginTurn)
        {
            if (effect.DecrementDuration())
            {
                statusEffects.Remove(effect);
            }
        }
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

            ReceiveDamage(rawDamage, attackData.damageType, attackData.armorPenetration);
        }
        else  // Attack is Evaded
        {
            
        }
    }

    /// <summary>
    /// Deals the given amount of damage to this unit.
    /// </summary>
    /// <param name="rawAmount">The raw amount of Damage from the attack (post-Critical Hit and Offense modifiers on the attacker, but pre-Defense reduction from the target).</param>
    /// <param name="type">The attack's Damage Type.</param>
    /// <param name="armorPenetration">The attack's Armor Penetration.</param>
    private void ReceiveDamage(float rawAmount, DamageType type, float armorPenetration)
    {
        // Compute reduction from Defense
        float selectedDefense = (type == DamageType.Physical) ? CurrentStats.physicalDefense : CurrentStats.specialDefense;
        float amount = rawAmount * (1f - selectedDefense / 100f);
        
        // Determine what proportion of damage should go to Armor and Health
        float amountToArmor = Mathf.Min(amount * (1f - armorPenetration), armor);
        float amountToHealth = amount - amountToArmor;
        AddHealth(amountToHealth * -1f);
        AddArmor(amountToArmor * -1f);

        Debug.Log($"{Data.name} received {amount} Damage (Health: {amountToHealth}, Armor: {amountToArmor}, raw: {rawAmount}). Remaining HP: {health + armor}.");
    }

    /// <summary>
    /// Determines whether the attack can target a given unit based on its targetable terrain types.
    /// </summary>
    /// <param name="target">The target unit.</param>
    /// <param name="attackData">The given attack.</param>
    /// <returns>Whether the attack is able to strike the target's terrain.</returns>
    public static bool IsTargetableTerrain(Unit target, AttackData attackData)
    {
        return attackData.targetableTerrains.Contains(target.Data.terrain);
    }

    #endregion

}
