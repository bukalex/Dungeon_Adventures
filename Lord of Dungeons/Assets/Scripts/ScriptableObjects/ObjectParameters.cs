using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Object Parameters", menuName = "ScriptableObjects/Object parameters")]

public class ObjectParameters : ScriptableObject
{
    public PlayerData playerData;
    public float colliderRadius = 1.0f;

    public GameObject dropPrefab;

    //Stats
    public float health = 30.0f;
    

    public float healthRestoreRate = 1.0f;


    public float attack = 3.0f;
    public float defense = 2.0f;
    public float specialDefense = 2.0f;

    public RuntimeAnimatorController animController;

    public enum ObjectType { POT, CHEST}

}
