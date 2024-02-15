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

    private bool beingChecked;

    void Awake()
    {
        animator.runtimeAnimatorController = lootableParameters.animController;
    }

    void Update()
    {

    }

    public void BeingLooted(bool isLooted)
    {

        
        if (chestInventory != null)
        {
            beingChecked = isLooted;
            chestInventory.SetActive(isLooted);
            UIManager.Instance.inventory.SetActive(isLooted);
            Debug.Log("Inventory chest should open.");
        }
        Open();
        beingChecked = false;

    }

    public float GetColliderRadius()
    {
        return lootableParameters.colliderRadius;
    }

    private void Open()
    {
        animator.SetBool("isOpen",beingChecked);

        Debug.Log("Opened Chest");
    }
}
