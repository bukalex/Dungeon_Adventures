using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class LootableController : MonoBehaviour
{
    [SerializeField]
    private LootableParameters lootableParameters;

    [SerializeField]
    private GameObject chestInventory;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator.runtimeAnimatorController = lootableParameters.animController;
    }

    void Update()
    {

    }

    public void InteractWithPlayer(bool isActive)
    {

        Open();
        if (chestInventory != null)
        {
            chestInventory.SetActive(isActive);
        }
        Debug.Log("Inventory should open.");


    }

    public float GetColliderRadius()
    {
        return lootableParameters.colliderRadius;
    }

    private void Open()
    {
        animator.SetTrigger("isOpen");

        Debug.Log("Opened Chest");
    }
}
