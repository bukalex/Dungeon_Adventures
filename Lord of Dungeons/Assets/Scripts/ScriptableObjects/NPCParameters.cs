using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Parameters", menuName = "ScriptableObjects/NPC parameters")]
public class NPCParameters : ScriptableObject
{
    public PlayerData playerData;
    public float colliderRadius = 1.0f;
    public float timer = 0;

    //Type
    public NPCType type = NPCType.TRADER;

    //Animator controller
    public RuntimeAnimatorController animController;

    //Enums
    public enum NPCType { TRADER, BLACKSMITH, WIZARD, BANKER, HOMELESS, TELEPORT }

    public void SellItem(int price)
    {
       //if (playerData.resources[Item.MaterialType.Coin] >= price)
       //{
       //    playerData.resources[Item.MaterialType.Coin] -= price;
       //    Debug.Log("Sold");
       //}
    }
}
