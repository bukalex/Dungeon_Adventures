using Assets.Enums.ItemEnums;
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
    public int materialID;
    public int unitsPerStack;

    [Header("For equipment")]
    public BodyPartName bodyPart;
    public WeaponType weaponType;
    public int atlasID;

    [Header("Inventory enums")]
    public ItemTag tag;
    public ItemType itemType;
    public CoinType materialType;
    [Header("Buffs")]
    public float addHP;
    public float addMP;
    public float addSPD;
    public float addStamina;

    public float restoreHP;
    public float restoreMP;
    public float restoreSPD;
    public float restoreStamina;

    [Header("Additions")]
    public float addAttack;
    public float addDefense;
    public float addSpecialAttack;
    public float addSpecialDefense;

    [Header("Multipliers")]
    public float increseAttack = 1;
    public float increaseDefense = 1;
    public float increseSpecialAttack = 1;
    public float increaseSpecialDefense = 1;

    [Range(0f, 30f)]
    [Header("Change the name of the variable to 'duration' later")]
    public float cooldown; //duration

    [Header("Other")]
    [Tooltip("Ability for assigning to a book")]
    public Ability ability;
    [Tooltip("Items array for lamp")]
    public Item[] items;
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

    public enum ItemType { Potion, Material, Gear, Weapon, Artifact, Treasure, Food, Trash, Spell, MonsterLoot, Coin}
    public enum ItemTag { Item, Helmet, Chestplate, Gloves, Boots, Sword, Gem, CraftMaterial}
    public enum CoinType { OFF, GoldenCoin, SilverCoin, CopperCoin }

    public int GetCraftItemID(Item item)
    {
        return item.materialID;
    }

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
