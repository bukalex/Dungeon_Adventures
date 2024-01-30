using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpPoison : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            GeneralCharacterController generalCharacterController = other.gameObject.GetComponent<GeneralCharacterController>();
            if (generalCharacterController != null)
            {
                generalCharacterController.health += 30;
            }
            Destroy(gameObject);
        }
    }
}
