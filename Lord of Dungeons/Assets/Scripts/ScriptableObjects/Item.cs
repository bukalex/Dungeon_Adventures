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

    public string description;
    [Range(0f, 30f)]
    public float cooldown;

    [Header("Other")]
    [Tooltip("Ability for assigning to a book")]
    public Ability ability;
    [Tooltip("Items array for lamp")]
    public Item[] items;

    [Header("Price")]
    public int GoldenCoin;
    public int SilverCoin;
    public int CopperCoin;
    

    [Header("Features")]
    public bool isStackable = false;
    public bool isUsable = false;

    [Header("Properties")]
    public Sprite image;
    public RuntimeAnimatorController animController;

    public enum ItemType { Potion, CraftMaterials, UpgrageMaterials, Gear, Weapon, Gems, Artifacts, Treasures, Food, Trash, Spell, MonsterLoot}
    public enum CoinType { GoldenCoin, SilverCoin, CopperCoin }
}
