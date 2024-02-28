using Assets.Scripts.InventoryElements;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item")]
public class ItemParam : ScriptableObject
{
    public List<Item> newItems = new List<Item>();
    [Header("Data")]
    public PlayerData playerData;
    public string name;
    public string description;
    public int unitsPerStack;
    
    public ItemTag tag;
    public ItemType itemType;
    
    [Header("Other")]
    [Tooltip("Ability for assigning to a book")]
    public Ability ability;
    [Range(0f, 101f)]
    [Tooltip("A rarity scale for randomizer, 0 is unobtainable. 101 is guranteed")]
    public float rarity;
    
    [Header("Store Price")]
    public int GoldenCoin;
    public int SilverCoin;
    public int CopperCoin;
    
    [Header("Selling Price")]
    public int GoldenCoins;
    public int SilverCoins;
    public int CopperCoins;
    
    [Header("Properties")]
    public Sprite image;
    public RuntimeAnimatorController animController;
    
    public enum ItemType { Potion, Material, Gear, Weapon, Artifact, Treasure, Food, Trash, Spell, MonsterLoot, Coin}
    public enum ItemTag { Item, Helmet, Chestplate, Gloves, Boots, Sword, Gem}
    public enum CoinType { OFF, GoldenCoin, SilverCoin, CopperCoin }
    
    public string GetItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Potion:
                return "Potion";
    
            case ItemType.Spell:
                return ItemType.Spell.ToString();
    
            case ItemType.MonsterLoot: 
                return "Monster Loot";
    
            case ItemType.Material:
                return "Material";
    
            case ItemType.Gear:
                return "Gear";
    
            case ItemType.Treasure:
                return "Treasure";
        }
    
        return null;
    }



}
