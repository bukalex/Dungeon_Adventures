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
                playerData.resources[ItemParam.CoinType.GoldenCoin] += 1000;
                playerData.resources[ItemParam.CoinType.SilverCoin] += 1000;
                playerData.resources[ItemParam.CoinType.CopperCoin] += 1000;
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
