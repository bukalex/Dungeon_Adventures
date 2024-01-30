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
           
            GeneralCharacterController playerController = other.gameObject.GetComponent<GeneralCharacterController>();
            if (playerController != null)
            {
                playerController.uiManager.UpdateScore(1);
            }
            Destroy(gameObject);  
        }
    }
}
