using UnityEngine;

[CreateAssetMenu(fileName = "New Lootable Parameters", menuName = "ScriptableObjects/Lootable parameters")]

public class LootableParameters : ScriptableObject
{
    public PlayerData playerData;
    public float colliderRadius = 1.0f;

    //Animator controller
    public RuntimeAnimatorController animController;
}
