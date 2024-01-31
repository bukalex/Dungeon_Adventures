using System.Collections;
using System.Collections.Generic;
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

    [Header("Only UI")]
    public bool isStackable = true;
    public bool isUsable = true;

    [Header("Both")]
    public Sprite image;
    public RuntimeAnimatorController animController;

    public enum ItemType { Poison, Material, Gear, Weapons}
    public enum MaterialType { Coin, Iron, Rock }
}
