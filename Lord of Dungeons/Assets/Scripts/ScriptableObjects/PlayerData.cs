using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "ScriptableObjects/Player data")]
public class PlayerData : ScriptableObject, IAttackObject, IDefenseObject
{
    [Header("Initial stats")]
    public float maxHealth = 100.0f;
    public float maxMana = 50.0f;
    public float maxStamina = 50.0f;
    public float initialSpeed = 0.75f;

    public float initialAttack = 3.0f;
    public float initialSpecialAttack = 3.0f;
    public float initialDefense = 2.0f;
    public float initialSpecialDefense = 2.0f;

    [Header("Run-time stats")]
    public float health = 100.0f;
    public float mana = 50.0f;
    public float stamina = 50.0f;
    public float speed = 0.75f;
    public float sprintFactor = 1.5f;
    public float slowWalkFactor = 0.75f;

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

    [Header("Others")]
    public float npcDetectionRadius = 0.75f;
    public float lootableDetectionRadius = 0.75f;
    public float colliderRadius = 1.0f;
    public Vector3 position = Vector3.zero;
    public Vector3 attackDirection = Vector3.down;
    public Transform transform;
    public bool isStunned = false;
    public Dictionary<CharacterType, Dictionary<BattleManager.AttackButton, AttackParameters>> attacks = null;

    //Type
    public CharacterType type = CharacterType.WARRIOR;

    //Animator controller
    public RuntimeAnimatorController animController;

    //Resources
    public Dictionary<Item.CoinType, int> resources = new Dictionary<Item.CoinType, int>();

    //Enums
    public enum CharacterType { WARRIOR, ARCHER, WIZARD }

    public void SetStats()
    {
        health = maxHealth;
        mana = maxMana;
        stamina = maxStamina;

        attack = initialAttack;
        specialAttack = initialSpecialAttack;
        defense = initialDefense;
        specialDefense = initialSpecialDefense;

        speed = initialSpeed;
    }

    public void SetDictionaries()
    {
        //Set resources
        resources.Clear();
        resources.Add(Item.CoinType.GoldenCoin, 0);
        resources.Add(Item.CoinType.SilverCoin, 0);
        resources.Add(Item.CoinType.CopperCoin, 0);
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public void RestoreStats()
    {
        health = Mathf.Clamp(health + healthRestoreRate * Time.deltaTime, 0, maxHealth);
        if (!isUsingMana)
        {
            mana = Mathf.Clamp(mana + manaRestoreRate * Time.deltaTime, 0, maxMana);
        }
        if (!isUsingStamina)
        {
            stamina = Mathf.Clamp(stamina + staminaRestoreRate * Time.deltaTime, 0, maxStamina);
        }
    }

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
