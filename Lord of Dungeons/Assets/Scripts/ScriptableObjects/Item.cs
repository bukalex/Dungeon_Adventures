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

    [Header("Only Gameplay")]
    public TileBase tile;
    public ItemType itemType;
    public MaterialType materialType;
    public Vector2Int range = new Vector2Int(5, 4);
    public int price;
    public int addHP;
    public int addSPD;
    public int addStamina;
    public int removeHP;
    public string description;
    [Range(0f, 30f)]
    public float cooldown;

    [Header("Only UI")]
    public bool isStackable = false;
    public bool isUsable = false;

    [Header("Both")]
    public Sprite image;
    public RuntimeAnimatorController animController;

    public enum ItemType { Poison, Material, Gear, Weapons}
    public enum MaterialType { GoldenCoin, SilverCoin, CopperCoin }
}
