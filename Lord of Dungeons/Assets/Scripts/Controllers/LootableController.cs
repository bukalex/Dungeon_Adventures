using TMPro;
using UnityEngine;

public class LootableController : MonoBehaviour, IInteractable
{
    [SerializeField]
    private LootableParameters lootableParameters;
    [SerializeField]
    private int chestIndex;
    private GameObject chestInventory;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private GameObject interactIconPrefab;
    private GameObject interactIcon;

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
    }

    public float GetColliderRadius()
    {
        return lootableParameters.colliderRadius;
    }

    private void Open()
    {
        animator.SetBool("isOpen", beingChecked);

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
