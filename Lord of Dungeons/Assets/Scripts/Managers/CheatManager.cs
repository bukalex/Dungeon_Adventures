using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                playerData.resources[Item.MaterialType.Coin] += 1000;
            }
        }
    }
}
