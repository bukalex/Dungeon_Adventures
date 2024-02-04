using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPCParameters;

[CreateAssetMenu(fileName = "New Lootable Parameters", menuName = "ScriptableObjects/Lootable parameters")]

public class LootableParameters : ScriptableObject
{
    public PlayerData playerData;
    public float colliderRadius = 1.0f;

    //Animator controller
    public RuntimeAnimatorController animController;
}
