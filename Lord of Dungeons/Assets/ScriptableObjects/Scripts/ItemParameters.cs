using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Parameters", menuName = "Item parameters")]
public class ItemParameters : ScriptableObject
{
    public PlayerData playerData;

    //Stats
    public float damageMult;
    public float healthMult;
    public int price;
    public bool isRecyclable;
    public bool isSellable;


    //Image
    public Sprite sprite;

    //Animator controller
    public RuntimeAnimatorController animController;

    //Types
    public ItemCategory category = ItemCategory.RESOURCES;
    public ResourceType resourceType = ResourceType.COIN;
    public EquipmentType equipmentType ;
    public itemClass items;

    //Enums
    public enum ItemCategory { RESOURCES, EQUIPMENT }
    public enum ResourceType { COIN, WOOD, ROCK }
    public enum EquipmentType { SWORD }

    public enum itemClass { OFF, A, B, C, D, E}
}
