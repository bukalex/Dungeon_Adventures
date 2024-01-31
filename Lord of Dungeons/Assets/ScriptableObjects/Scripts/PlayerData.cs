using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "ScriptableObjects/Player data")]
public class PlayerData : ScriptableObject
{
    public Vector3 position = Vector3.zero;
    public float colliderRadius = 1.0f;

    //Stats
    public float maxHealth = 100.0f;
    public float maxMana = 50.0f;
    public float maxStamina = 50.0f;

    public float health = 100.0f;
    public float mana = 50.0f;
    public float stamina = 50.0f;
    public float speed = 0.75f;

    public float healthRestoreRate = 1.0f;
    public float manaRestoreRate = 1.0f;
    public float staminaRestoreRate = 1.0f;

    public float attack = 10.0f;
    public float specialAttack = 10.0f;
    public float defense = 10.0f;
    public float specialDefense = 10.0f;

    public float npcDetectionRadius = 0.75f;

    public bool isUsingMana = false;
    public bool isUsingStamina = false;

    //Type
    public CharacterType type = CharacterType.WARRIOR;

    //Animator controller
    public RuntimeAnimatorController animController;

    //Resources
    public Dictionary<ItemParameters.ResourceType, int> resources = new Dictionary<ItemParameters.ResourceType, int>();

    //Attacks
    public Dictionary<AttackButton, AttackType> attacksByType = new Dictionary<AttackButton, AttackType>();
    public Dictionary<AttackButton, float> attacksByDamage = new Dictionary<AttackButton, float>();
    public Dictionary<AttackButton, float> attacksByMana = new Dictionary<AttackButton, float>();
    public Dictionary<AttackButton, float> attacksByStamina = new Dictionary<AttackButton, float>();
    public Dictionary<AttackButton, float> attacksByRange = new Dictionary<AttackButton, float>();
    public Dictionary<AttackButton, float> attacksByCooldown = new Dictionary<AttackButton, float>();

    //Enums
    public enum CharacterType { WARRIOR, ARCHER, WIZARD }
    public enum AttackButton { NONE, LMB, RMB, SHIFT }
    public enum AttackType { BASIC, SPECIAL }

    public void SetDictionaries()
    {
        //Set resources
        resources.Clear();
        resources.Add(ItemParameters.ResourceType.COIN, 0);
        resources.Add(ItemParameters.ResourceType.WOOD, 0);
        resources.Add(ItemParameters.ResourceType.ROCK, 0);

        //Set attacks
        attacksByDamage.Clear();
        attacksByType.Clear();
        attacksByMana.Clear();
        attacksByStamina.Clear();
        attacksByRange.Clear();
        attacksByCooldown.Clear();

        switch (type)
        {
            case CharacterType.WARRIOR:
                attacksByDamage.Add(AttackButton.LMB, attack * 1.5f);
                attacksByType.Add(AttackButton.LMB, AttackType.BASIC);
                attacksByMana.Add(AttackButton.LMB, 0.0f);
                attacksByStamina.Add(AttackButton.LMB, 2.5f);
                attacksByRange.Add(AttackButton.LMB, 0.7f);
                attacksByCooldown.Add(AttackButton.LMB, 0.625f);

                attacksByMana.Add(AttackButton.RMB, 2.5f);
                attacksByStamina.Add(AttackButton.RMB, 0.0f);

                attacksByMana.Add(AttackButton.SHIFT, 0.0f);
                attacksByStamina.Add(AttackButton.SHIFT, 1.0f);
                break;

            case CharacterType.ARCHER:
                break;

            case CharacterType.WIZARD:
                break;
        }
    }

    public bool isAlive()
    {
        return health > 0;
    }

    public void DealDamage(EnemyParameters.AttackType attackType, float damage)
    {
        Debug.Log("Player was hit. Damage: " + damage);

        health -= damage;
    }

    public bool AffordAttack(AttackButton attackButton, bool isPerFrame = false)
    {
        float multiplier = 1.0f;
        if (isPerFrame)
        {
            multiplier = Time.deltaTime;
        }

        if (attacksByMana[attackButton] != 0)
        {
            if (attacksByMana[attackButton] <= mana)
            {
                isUsingMana = true;
            }
            else
            {
                return false;
            }
        }
        if (attacksByStamina[attackButton] != 0)
        {
            if (attacksByStamina[attackButton] <= stamina)
            {
                isUsingStamina = true;
            }
            else
            {
                return false;
            }
        }

        if (isUsingMana)
        {
            mana = Mathf.Clamp(mana - attacksByMana[attackButton] * multiplier, 0, maxMana);
        }
        if (isUsingStamina)
        {
            stamina = Mathf.Clamp(stamina - attacksByStamina[attackButton] * multiplier, 0, maxStamina);
        }

        return true;
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
}
