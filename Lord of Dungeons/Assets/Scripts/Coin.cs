using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           
            WarriorController playerController = other.gameObject.GetComponent<WarriorController>();
            if (playerController != null)
            {
                playerController.UpdateScore(1);
            }
            Destroy(gameObject);  
        }
    }
}
