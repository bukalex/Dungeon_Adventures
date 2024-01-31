using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "New Enemy Parameters", menuName = "ScriptableObjects/Enemy parameters")]
public class EnemyParameters : ScriptableObject
{
    public PlayerData playerData;
    public float colliderRadius = 1.0f;

    //Stats
    public float health = 100.0f;
    public float mana = 50.0f;
    public float stamina = 50.0f;
    public float speed = 0.6f;

    public float healthRestoreRate = 1.0f;
    public float manaRestoreRate = 1.0f;
    public float staminaRestoreRate = 1.0f;

    public float attack = 3.0f;
    public float defense = 2.0f;
    public float specialDefense = 2.0f;

    public PlayerData.AttackType attackType = PlayerData.AttackType.BASIC;
    public float attackDamage = 15.0f;
    public float attackRange = 1.0f;
    public float attackCooldown = 1.0f;
    public float manaCost = 1.0f;
    public float staminaCost = 1.0f;

    public float detectionRadius = 3.5f;

    //Type
    public EnemyType type = EnemyType.GUARD;

    //Animator controller
    public RuntimeAnimatorController animController;

    //Projectile prefab
    public GameObject projectilePrefab;

    //Enums
    public enum EnemyType { GUARD, GHOST, RAT, BAT }
}
