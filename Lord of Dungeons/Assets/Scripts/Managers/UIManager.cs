using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private TMP_Text coinCounter;

    void Update()
    {
        //Update values
        coinCounter.text = playerData.resources[ItemParameters.ResourceType.COIN].ToString();
    }
}
