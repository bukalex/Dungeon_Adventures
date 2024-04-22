using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject slotPrefab;
    public Color selectedSlot, nonSelectedSlot;
    public Image image;

    public string slotTag;

    private void Awake()
    {
        //unselectSlot();
    }
    private void Start()
    {
    }
    public void selectSlot()
    {
        image.color = selectedSlot;
    }

    public void unselectSlot()
    {
        image.color = nonSelectedSlot;
    }
    public void OnDrop(PointerEventData eventData)  
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();

        //Check if item and slot have the same tag
        #region
        //Inventory slots
        if (!inventoryItem.isLocked)
        {
            if ((inventoryItem.InventoryItemPrefab.CompareTag("Item") && slotPrefab.CompareTag("Inventory") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Gem") && slotPrefab.CompareTag("Gem") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Gem") && slotPrefab.CompareTag("Inventory") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Sword") && slotPrefab.CompareTag("Sword") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Sword") && slotPrefab.CompareTag("Inventory") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Helmet") && slotPrefab.CompareTag("Helmet") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Helmet") && slotPrefab.CompareTag("Inventory") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Chestplate") && slotPrefab.CompareTag("Chestplate") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Chestplate") && slotPrefab.CompareTag("Inventory") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Gloves") && slotPrefab.CompareTag("Gloves") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Gloves") && slotPrefab.CompareTag("Inventory") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Boots") && slotPrefab.CompareTag("Boots") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Boots") && slotPrefab.CompareTag("Inventory") && transform.childCount == 0) ||
            (inventoryItem.item.itemType == Item.ItemType.Artifact && slotPrefab.CompareTag("Artifact") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("CraftMaterial") && slotPrefab.CompareTag("CraftMaterial") && transform.childCount == 0) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("CraftMaterial") && slotPrefab.CompareTag("Inventory") && transform.childCount == 0)
            )
            {
                inventoryItem.parentAfterDrag = transform;
            }
        }
        #endregion

        if (transform.parent.GetComponentInParent<PotWindow>() != null)
        {
            StartCoroutine(IngridientEvent(0.1f));
        }
    }
    public IEnumerator onItemDescribe(float interval)
    {
        yield return new WaitForSeconds(interval);
        InventoryManager.Instance.ItemDescription.SetActive(true);
    }
    private IEnumerator IngridientEvent(float timer)
    {
        yield return new WaitForSeconds(timer);

        transform.parent.GetComponentInParent<PotWindow>().ingridientInSlot = transform.GetComponentInChildren<InventoryItem>().item;
        transform.parent.GetComponentInParent<PotWindow>().onIngridientAdd.Invoke();
    }
}
