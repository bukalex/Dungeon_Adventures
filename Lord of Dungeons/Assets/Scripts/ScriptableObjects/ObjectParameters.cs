using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Object Parameters", menuName = "ScriptableObjects/Object parameters")]

public class ObjectParameters : ScriptableObject, IDefenseObject
{
    [Header("Initial stats")]
    public float health = 30.0f;

    [Header("Run-time stats")]
    public float attack = 3.0f;
    public float specialAttack = 3.0f;
    public float defense = 2.0f;
    public float specialDefense = 2.0f;

    [Header("Others")]
    public PlayerData playerData;
    public float colliderRadius = 1.0f;
    public Vector3 position;
    //public GameObject dropPrefab;
    public RuntimeAnimatorController animController;

    public enum ObjectType { POT, CHEST}

    //Interfaces
    #region
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

    public float DealDamage(float damage)
    {
        health -= damage;
        return damage;
    }

    public void DisableStun()
    {
        
    }

    public Vector3 GetPosition()
    {
        return position;
    }
    #endregion
}
