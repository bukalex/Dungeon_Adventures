using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Enemy Parameters", menuName = "ScriptableObjects/Enemy parameters")]
public class EnemyParameters : ScriptableObject, IAttackObject, IDefenseObject
{
    [Header("Initial stats")]
    public float maxHealth = 100.0f;
    public float maxMana = 50.0f;
    public float maxStamina = 50.0f;

    [Header("Run-time stats")]
    public float health = 100.0f;
    public float mana = 50.0f;
    public float stamina = 50.0f;
    public float speed = 0.6f;

    public float attack = 3.0f;
    public float specialAttack = 3.0f;
    public float defense = 2.0f;
    public float specialDefense = 2.0f;

    [Header("Restore values")]
    public float healthRestoreRate = 1.0f;
    public float manaRestoreRate = 1.0f;
    public float staminaRestoreRate = 1.0f;

    public bool isUsingMana = false;
    public bool isUsingStamina = false;

    [Header("Obstacle avoidance")]
    public float whiskerLength = 2.0f;
    public float whiskerAngle = 10.0f;
    public float rotationSpeed = 60.0f;

    [Header("Others")]
    public PlayerData playerData;
    public float colliderRadius = 1.0f;
    public float detectionRadius = 3.5f;
    public bool isBoss = false;
    public bool isStunned = false;
    public Vector3 position = Vector3.zero;
    public Vector3 attackDirection = Vector3.down;
    public Transform transform;

    //Type
    public EnemyType type = EnemyType.GUARD;

    //Animator controller
    public RuntimeAnimatorController animController;

    //Projectile prefab
    public GameObject projectilePrefab;

    //Enums
    public enum EnemyType { GUARD, GHOST, RAT, BAT }

    //Interfaces
    #region
    public float GetAttackValue(BattleManager.AttackType attackType)
    {
        switch (attackType)
        {
            case BattleManager.AttackType.BASIC:
                return attack;

            case BattleManager.AttackType.SPECIAL:
                return specialAttack;
        }
        
        return 0;
    }

    public float GetMana(bool max = false)
    {
        if (max)
        {
            return maxMana;
        }
        else
        {
            return mana;
        }
    }

    public float GetStamina(bool max = false)
    {
        if (max)
        {
            return maxStamina;
        }
        else
        {
            return stamina;
        }
    }

    public void SetMana(float mana)
    {
        this.mana = mana;
    }

    public void SetStamina(float stamina)
    {
        this.stamina = stamina;
    }

    public bool GetIsUsingMana()
    {
        return isUsingMana;
    }

    public bool GetIsUsingStamina()
    {
        return isUsingStamina;
    }

    public void SetIsUsingMana(bool isUsingMana)
    {
        this.isUsingMana = isUsingMana;
    }

    public void SetIsUsingStamina(bool isUsingStamina)
    {
        this.isUsingStamina = isUsingStamina;
    }

    public float GetDefenseValue(BattleManager.AttackType attackType)
    {
        switch (attackType)
        {
            case BattleManager.AttackType.BASIC:
                return defense;

            case BattleManager.AttackType.SPECIAL:
                return specialDefense;
        }

        return 0;
    }

    public void DealDamage(float damage)
    {
        health -= damage;
    }

    public void DisableStun()
    {
        isStunned = false;
    }
    #endregion
}
