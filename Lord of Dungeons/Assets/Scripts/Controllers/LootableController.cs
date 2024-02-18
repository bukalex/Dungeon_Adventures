using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class LootableController : MonoBehaviour
{
    [SerializeField]
    private LootableParameters lootableParameters;
    [SerializeField]
    public GameObject chestInventory;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private bool beingChecked;

    void Awake()
    {
        animator.runtimeAnimatorController = lootableParameters.animController;

    }

    private void Start()
    {
        chestInventory.SetActive(false);
    }
    public void BeingLooted(bool isLooted)
    {

        
        if (chestInventory != null)
        {
            beingChecked = isLooted;
            chestInventory.SetActive(isLooted);
            UIManager.Instance.InventorySlots.SetActive(isLooted);
            
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
