using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class UIManager : MonoBehaviour
{
    [SerializeField] Text scoreText;

    public int score = 0;  // Score number of coin

    public void UpdateScore(int coinValue)
    {
        score += coinValue;
        Debug.Log(scoreText);
        scoreText.text = score.ToString();
    }
}
