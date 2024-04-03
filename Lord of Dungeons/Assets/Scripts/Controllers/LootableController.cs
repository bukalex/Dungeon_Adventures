using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class LootableController : MonoBehaviour, IInteractable
{
    [SerializeField]
    private LootableParameters lootableParameters;
    [SerializeField]
    private int chestIndex;
    public GameObject chestInventory;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private GameObject interactIconPrefab;
    private GameObject interactIcon;
    [Header("Trapped Chest Properties")]
    [SerializeField]
    private bool isTrapped;
    private bool triggered = false;
    [SerializeField]
    public GameObject encounter;
    [SerializeField]
    public List<GameObject> enemies = new List<GameObject>();
    public float remaining;

    private bool beingChecked;

    void Awake()
    {
        animator.runtimeAnimatorController = lootableParameters.animController;
    }

    private void Start()
    {
        chestInventory = UIManager.Instance.ChestUIs[chestIndex];
        chestInventory.SetActive(false);
        interactIcon = Instantiate(interactIconPrefab, Camera.main.WorldToScreenPoint(transform.position + Vector3.up), Quaternion.identity, GameObject.FindGameObjectWithTag("MainCanvas").transform);
        interactIcon.transform.SetSiblingIndex(0);
        interactIcon.SetActive(false);
    }
    public void BeingLooted(bool isLooted)
    {
        if (isTrapped == false)
        {
            if (chestInventory != null)
            {
                beingChecked = isLooted;
                chestInventory.SetActive(isLooted);
                UIManager.Instance.InventorySlots.SetActive(isLooted);

                Debug.Log("Inventory chest should open.");
            }
            Open();
            //beingChecked = false;
            UIManager.Instance.npcWindowActive = isLooted;
            if (!isLooted) InventoryManager.Instance.ItemDescription.SetActive(false);
        }
        else
        {
            
            if(isTrapped == true && triggered == false)
            {
                encounter.SetActive(true);
                triggered = true;
                Debug.Log("Trap has been triggered?: " + triggered);
            }
            enemies.RemoveAll(enemy => enemy == null);
            remaining = enemies.Count;

            if (remaining <= 0)
            {
                isTrapped = false;
            }

            
        }
        
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

    public void ShowButton()
    {
        if (interactIcon != null)
        {
            interactIcon.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
            interactIcon.GetComponentInChildren<TMP_Text>().text = UIManager.Instance.textKeys[15].text;
            interactIcon.SetActive(true);
        }
    }

    public void HideButton()
    {
        if (interactIcon != null) interactIcon.SetActive(false);
    }
}
