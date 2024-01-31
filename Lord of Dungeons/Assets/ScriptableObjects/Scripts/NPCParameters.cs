using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Parameters", menuName = "ScriptableObjects/NPC parameters")]
public class NPCParameters : ScriptableObject
{
    public PlayerData playerData;
    public float colliderRadius = 1.0f;

    //Type
    public NPCType type = NPCType.TRADER;

    //Enums
    public enum NPCType { TRADER, BLACKSMITH }

    public void SellItem(int price)
    {
        if (playerData.resources[ItemParameters.ResourceType.COIN] >= price)
        {
            playerData.resources[ItemParameters.ResourceType.COIN] -= price;
            Debug.Log("Sold");
        }
    }
}
