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
                playerData.resources[Item.CoinType.GoldenCoin] += 100;
                playerData.resources[Item.CoinType.SilverCoin] += 100;
                playerData.resources[Item.CoinType.CopperCoin] += 100;
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                playerData.attack += 100;
                playerData.specialAttack += 100;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                playerData.defense += 100;
                playerData.specialDefense += 100;
            }
        }
    }
}
