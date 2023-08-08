using System;
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
    public List<ActiveAbility> ActiveAbilities { get; set; } = new List<ActiveAbility>();
    public List<PassiveAbility> PassiveAbilities { get; set; } = new List<PassiveAbility>();
    public List<StatusEffect> StatusEffects { get; set; } = new List<StatusEffect>();
    private List<StatusEffect> statusEffectsBeginTurn;
    private List<ActiveAbility> cooldownAbilitiesBeginTurn;

    // Get current stats from base data plus all Status Effects and modifiers on this unit
    public Stats CurrentStats
    {
        get
        {
            return Data.stats.ApplyModifiers(this);
        }
    }
    
    // Get current state from all modifiers
    public State CurrentState
    {
        get
        {
            return State.ApplyStatusEffects(StatusEffects);
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
        health = CurrentStats.maxHealth;
        armor = CurrentStats.maxArmor;
        turnMeter = 0f;
        
        // Active Abilities
        foreach (ActiveAbilityData abilityData in Data.activeAbilities)
        {
            ActiveAbilities.Add(new ActiveAbility(abilityData));
        }
        
        // Passive Abilities
        foreach (PassiveAbilityData abilityData in Data.passiveAbilities)
        {
            PassiveAbilities.Add(new PassiveAbility(abilityData, this));
        }

        // Connect unit to its tile
        GridManager.instance.ConnectUnitToTile(this);

        // Visual
        display.UpdateHealthArmorBar(health, CurrentStats.maxHealth, armor, CurrentStats.maxArmor);
        display.UpdateTurnMeterBar(turnMeter);
    }

    /// <summary>
    /// Generates a list of units belonging to the given group from this unit's perspective.
    /// </summary>
    /// <param name="group">The given group.</param>
    /// <returns>List of units belonging to the given group.</returns>
    public List<Unit> GetGroup(UnitGroup group)
    {
        List<Unit> units = new List<Unit>();

        switch (group)
        {
            case UnitGroup.Self:
                units = new List<Unit>{ this };
                break;

            case UnitGroup.Allies:
                units = new List<Unit>(Allies);
                break;

            case UnitGroup.Enemies:
                units = new List<Unit>(Enemies);
                break;

            case UnitGroup.OtherAllies:
                units = new List<Unit>(Allies);
                units.Remove(this);
                break;

            case UnitGroup.AllUnits:
                units = new List<Unit>(GameManager.instance.AllUnits);
                break;

            default:
                break;
        }

        return units;
    }

    #region Turn

    // Evaluate whether this unit has at least 100% Turn Meter and is therefore ready to take a turn.
    public bool ReadyForTurn
    {
        get
        {
            return turnMeter >= 100f;
        }
    }

    /// <summary>
    /// Begins the unit's turn.
    /// </summary>
    /// <returns>Whether the unit's turn should be immediately skipped.</returns>
    public bool BeginTurn()
    {
        // Track Status Effects which are present at the beginning of the turn to decrement on turn end
        statusEffectsBeginTurn = new List<StatusEffect>(StatusEffects);
        
        // Track cooldowns which are active at the beginning of the turn to decrement on turn end
        cooldownAbilitiesBeginTurn = new List<ActiveAbility>();
        foreach (ActiveAbility ability in ActiveAbilities)
        {
            if (ability.Cooldown > 0)
            {
                cooldownAbilitiesBeginTurn.Add(ability);
            }
        }

        // Broadcast
        EventManager.instance.TurnBegin(this);

        return CurrentState.skipTurn;
    }

    /// <summary>
    /// Ends the unit's turn
    /// </summary>
    public void EndTurn()
    {
        ResetTurnMeter();
        DecrementStatusEffects();
        DecrementCooldowns();

        // Broadcast
        EventManager.instance.TurnEnd(this);
    }

    #endregion

    #region Turn Meter

    /// <summary>
    /// Updates the unit's Turn Meter by the given amount. TM may exceed 100%.
    /// </summary>
    /// <param name="amount">The amount by which to increase the unit's TM. Can be negative (i.e. TM loss).</param>
    /// <param name="natural">Whether this is natural Turn Meter from the turn routine.</param>
    public void AddTurnMeter(float amount, bool natural)
    {
        turnMeter += amount;
        display.UpdateTurnMeterBar(turnMeter);  // Visual
    }
    
    /// <summary>
    /// Removes 100% Turn Meter from this unit to soft-reset it after taking a turn. Overflow TM may remain as a residual.
    /// </summary>
    private void ResetTurnMeter()
    {
        AddTurnMeter(-100f, true);
        display.UpdateTurnMeterBar(turnMeter);  // Visual
    }
    
    /// <summary>
    /// Updates the unit's Turn Meter based on their Speed (natural TM generation). TM is generated in "frames", i.e. 1% of Speed in percentage points at a time.
    /// </summary>
    public void GenerateTurnMeterFromSpeed()
    {
        float amount = Mathf.Min(CurrentStats.speed * 0.01f, 100f - turnMeter);  // If TM is less than 1 "frame" away from 100%, give partial frame
        AddTurnMeter(amount, true);
    }
    
    #endregion

    #region Health

    /// <summary>
    /// Add to this unit's current Health.
    /// </summary>
    /// <param name="amount">Amount of Health to add.</param>
    public void AddHealth(float amount)
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
    public List<StatusEffect> Buffs
    {
        get
        {
            List<StatusEffect> buffs = new List<StatusEffect>();
            foreach (StatusEffect effect in StatusEffects)
            {
                if (effect.Data.type == StatusEffectType.Buff)
                {
                    buffs.Add(effect);
                }
            }
            return buffs;
        }
    }

    public List<StatusEffect> Debuffs
    {
        get
        {
            List<StatusEffect> debuffs = new List<StatusEffect>();
            foreach (StatusEffect effect in StatusEffects)
            {
                if (effect.Data.type == StatusEffectType.Debuff)
                {
                    debuffs.Add(effect);
                }
            }
            return debuffs;
        }
    }

    /// <summary>
    /// Checks whether the given unit should receive the given Status Effect, and adds it if so.
    /// </summary>
    /// <param name="source">The unit applying the Status Effect.</param>
    /// <param name="effectApplier">Details about the given Status Effect to apply.</param>
    public void ReceiveStatusEffect(Unit source, StatusEffectApplier effectApplier)
    {   
        // Do nothing if the effect fails to apply entirely
        int chanceToApply = effectApplier.chance;
        if (!(UnityEngine.Random.Range(0, 100) < chanceToApply))
        {
            return;
        }

        StatusEffect effect = new StatusEffect(effectApplier.name, effectApplier.duration);  // Instantiate Status Effect

        if (effect.Data.type == StatusEffectType.Debuff)
        {
            // Resistance check
            int chanceToInflict = source.CurrentStats.potency - CurrentStats.resistance;
            if (!effectApplier.irresistible && !(UnityEngine.Random.Range(0, 100) < chanceToInflict))  // Effect is Resisted
            {
                return;
            }
        }

        // If the Status Effect is not stackable, search for it in the unit's existing Status Effects before adding it. Notice that even if a non-stackable Status Effect cannot be applied due it already existing, it still triggers Status Effect-related events.
        if (!effect.Data.stackable)
        {
            StatusEffect existingEffect = FindStatusEffect(effect.Data.name);

            // The unit already has this effect
            if (existingEffect != null)
            {
                // If the existing Status Effect has a longer duration, do nothing
                if (existingEffect.Duration > effect.Duration)
                {
                    return;
                }
                // Otherwise, remove the Status Effect so that it can be overwritten
                else
                {
                    RemoveStatusEffect(this, existingEffect, true);
                }
            }
        }

        // Broadcast
        if (effect.Data.type == StatusEffectType.Buff)
        {
            EventManager.instance.Buff(source, this, effectApplier);
        }
        else
        {
            EventManager.instance.Debuff(source, this, effectApplier);
        }

        effect.OnApply(this);  // Execute application effects
        StatusEffects.Add(effect);
    }

    /// <summary>
    /// Decrements all Status Effects which were already present on the unit at the beginning of this turn.
    /// </summary>
    public void DecrementStatusEffects()
    {
        foreach (StatusEffect effect in statusEffectsBeginTurn)
        {
            if (effect.DecrementDuration())
            {
                // Remove Status Effect if it has expired
                RemoveStatusEffect(this, effect, true);
            }
        }
    }

    /// <summary>
    /// Removes the given Status Effect instance from this unit.
    /// </summary>
    /// <param name="source">The unit removing the Status Effect.</param>
    /// <param name="effect">The Status Effect instance to remove.</param>
    /// <param name="natural">Whether this is a natural removal, i.e. the effect expired or was cleared intrinsically.</param>
    public void RemoveStatusEffect(Unit source, StatusEffect effect, bool natural)
    {
        effect.OnRemove();  // Execute removal effects
        StatusEffects.Remove(effect);

        // Broadcast if not a natural Status Effect removal (e.g. expired)
        if (!natural)
        {
            if (effect.Data.type == StatusEffectType.Buff)
            {
                EventManager.instance.BuffClear(source, this, effect.Data.name);
            }
            else
            {
                EventManager.instance.DebuffClear(source, this, effect.Data.name);
            }
        }
    }

    /// <summary>
    /// Removes the Status Effect with the given name on this unit.
    /// </summary>
    /// <param name="source">The unit removing the Status Effect.</param>
    /// <param name="effectName">The name of the Status Effect.</param>
    /// <param name="natural">Whether this is a natural removal, i.e. the effect expired or was cleared intrinsically.</param>
    public void RemoveStatusEffect(Unit source, string effectName, bool natural)
    {
        StatusEffect effectToRemove = FindStatusEffect(effectName);
        if (effectToRemove != null)
        {
            RemoveStatusEffect(source, effectToRemove, natural);
        }
    }

    /// <summary>
    /// Finds the Status Effect with the given name on this unit.
    /// </summary>
    /// <param name="effectName">The name of the Status Effect.</param>
    /// <returns>The Status Effect object.</returns>
    private StatusEffect FindStatusEffect(string effectName)
    {
        foreach (StatusEffect effect in StatusEffects)
        {
            if (effect.Data.name == effectName)
            {
                return effect;
            }
        }
        
        return null;
    }

    #endregion

    #region Cooldowns

    /// <summary>
    /// Decrements all cooldowns which were already active on the unit at the beginning of this turn.
    /// </summary>
    public void DecrementCooldowns()
    {
        foreach (ActiveAbility ability in cooldownAbilitiesBeginTurn)
        {
            ability.AddCooldown(-1);
        }
    }

    #endregion

    #region Attacks & Damage

    /// <summary>
    /// Performs the given attack against this unit and records the result.
    /// </summary>
    /// <param name="source">The unit performing the attack.</param>
    /// <param name="attackData">The given attack.</param>
    /// <param name="weight">The weight by which to multiply the attack's damage.</param>
    public void ReceiveAttack(Unit source, AttackData attackData, float weight)
    {
        // Terrain check; if the attack cannot strike the unit's terrain, ignore it completely
        if (!IsTargetableTerrain(attackData))
        {
            return;
        }

        // Get current attack stats (adding modifiers based on the attacker and the target)
        AttackStats currentAttackStats = attackData.stats.ApplyModifiers(source, this, attackData.modifiers);

        // Evasion check
        int chanceToHit = currentAttackStats.accuracy - CurrentStats.evasion;
        if (UnityEngine.Random.Range(0, 100) < chanceToHit)  // Attack hits
        {
            float rawDamage = currentAttackStats.damage * (currentAttackStats.offense + 1f) * weight;  // Notice Offense is zero-anchored; i.e. no modifiers means coefficient of 1

            // Critical Hit check
            int chanceToCrit = currentAttackStats.critChance - CurrentStats.critAvoidance;
            if (UnityEngine.Random.Range(0, 100) < chanceToCrit)
            {
                // Attack is a Critical Hit
                rawDamage *= currentAttackStats.critDamage;
                EventManager.instance.CriticalHit(source, this);
            }

            float damageDealt = ReceiveDamage(rawDamage, attackData.damageType, currentAttackStats.armorPenetration);
            EventManager.instance.Damage(source, this, damageDealt);
        }
        else  // Attack is Evaded
        {
            EventManager.instance.Evasion(source, this);
        }
    }

    /// <summary>
    /// Deals the given amount of damage to this unit.
    /// </summary>
    /// <param name="rawAmount">The raw amount of Damage from the attack (post-Critical Hit and Offense modifiers on the attacker, but pre-Defense reduction from the target).</param>
    /// <param name="type">The attack's Damage Type.</param>
    /// <param name="armorPenetration">The attack's Armor Penetration.</param>
    /// <returns>The total amount of Damage dealt (bounded above by how much remaining Health and Armor the unit had.</returns>
    private float ReceiveDamage(float rawAmount, DamageType type, float armorPenetration)
    {
        // Compute reduction from Defense
        float selectedDefense = (type == DamageType.Physical) ? CurrentStats.physicalDefense : CurrentStats.specialDefense;
        float amount = rawAmount * (1f - selectedDefense / 100f);  // Represents total post-Defense damage without considering the remaining values of those stats for this unit
        
        // Determine what proportion of damage should go to Armor and Health
        float amountToArmor = Mathf.Min(amount * (1f - armorPenetration), armor);
        float amountToHealth = Mathf.Min(health, amount - amountToArmor);
        AddHealth(amountToHealth * -1f);
        AddArmor(amountToArmor * -1f);
        return amountToHealth + amountToArmor;
    }

    #endregion

    #region Queries

    /// <summary>
    /// Queries whether this unit is targetable by the given attack based on its targetable terrain types.
    /// </summary>
    /// <param name="attackData">The given attack.</param>
    /// <returns>Whether the attack is able to strike the target's terrain.</returns>
    public bool IsTargetableTerrain(AttackData attackData)
    {
        return attackData.targetableTerrains.Contains(Data.terrain);
    }

    /// <summary>
    /// Queries whether this unit has all of the given tags.
    /// </summary>
    /// <param name="tags">A list of the tags the unit must have.</param>
    /// <returns>Whether the unit has all of the given tags.</returns>
    public bool HasTags(List<Tag> tags)
    {
        foreach (Tag tag in tags)
        {
            if (!Data.tags.Contains(tag))
            {
                return false;
            }
        }
        return true;
    }

    public bool HasEffects(List<string> effects)
    {
        foreach (string effectName in effects)
        {
            bool effectFound = false;

            foreach (StatusEffect effect in StatusEffects)
            {
                if (effect.Data.name == effectName)
                {
                    effectFound = true;
                    break;
                }
            }

            if (!effectFound)
            {
                return false;
            }
        }
        return true;
    }

    #endregion
}

public enum UnitGroup
{
    None,
    Self,
    Allies,
    Enemies,
    OtherAllies,
    AllUnits
}