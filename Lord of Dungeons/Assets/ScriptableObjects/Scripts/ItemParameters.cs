using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Parameters", menuName = "Item parameters")]
public class ItemParameters : ScriptableObject
{
    public PlayerData playerData;

    //Stats


    //Image
    public Sprite sprite;

    //Animator controller
    public RuntimeAnimatorController animController;

    //Types
    public ItemCategory category = ItemCategory.RESOURCES;
    public ResourceType resourceType = ResourceType.COIN;
    public EquipmentType equipmentType;

    //Enums
    public enum ItemCategory { RESOURCES, EQUIPMENT }
    public enum ResourceType { COIN, WOOD, ROCK }
    public enum EquipmentType { SWORD }
}
