using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    [Header("Only Gameplay")]
    public TileBase tile;
    public ItemType itemType;
    public Vector2Int range = new Vector2Int(5, 4);
    [Header("Only UI")]
    public bool isStackable = true;
    public bool isUsable = true;

    [Header("Both")]
    public Sprite image;

    public enum ItemType { Poison, Material, Gear, Sword}

}
