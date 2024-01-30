using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOManager : MonoBehaviour
{
    [SerializeField]
    private List<PlayerData> playerDatas;

    void Awake()
    {
        foreach (PlayerData playerData in playerDatas)
        {
            playerData.SetDictionaries();
        }
    }
}
