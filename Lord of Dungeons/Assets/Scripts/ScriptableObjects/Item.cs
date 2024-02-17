using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    [Header("Data")]
    public PlayerData playerData;
    public string name;
    public string description;
    public int unitsPerStack;

    [Header("Buffs")]
    //public TileBase tile;
    public ItemType itemType;
    public CoinType materialType;
    public float addHP;
    public float addMP;
    public float addSPD;
    public float addStamina;

    public float restoreHP;
    public float restoreMP;
    public float restoreSPD;
    public float restoreStamina;

    [Header("Multipliers")]
    public float increseAttack;
    public float increaseDamage;

    [Range(0f, 30f)]
    [Header("Change the name of the variable to 'duration' later")]
    public float cooldown; //duration

    [Header("Other")]
    [Tooltip("Ability for assigning to a book")]
    public Ability ability;
    [Tooltip("Items array for lamp")]
    public Item[] items;

    [Header("Store Price")]
    public int GoldenCoin;
    public int SilverCoin;
    public int CopperCoin;

    [Header("Selling Price")]
    public int GoldenCoins;
    public int SilverCoins;
    public int CopperCoins;

    [Header("Features")]
    public bool isStackable = false;
    public bool isUsable = false;
    public bool isLootable = false;
    public bool isPurchasable = false;
    public bool isCraftMaterial = false;
    public bool isUpgradeMaterial = false;

    [Header("Properties")]
    public Sprite image;
    public RuntimeAnimatorController animController;

    public enum ItemType { Potion, Material, Gear, Weapon, Gems, Artifacts, Treasures, Food, Trash, Spell, MonsterLoot}
    public enum CoinType { OFF, GoldenCoin, SilverCoin, CopperCoin }

}
