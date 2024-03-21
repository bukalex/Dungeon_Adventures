using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject slotPrefab;
    public Color selectedSlot, nonSelectedSlot;
    public Image image;

    private void Awake()
    {
        //unselectSlot();
    }
    private void Update()
    {
        //InventoryManager.Instance.ItemDescription.transform.position = Input.mousePosition + new Vector3(250f, 100f);
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
    }
    public IEnumerator onItemDescribe(float interval)
    {
        yield return new WaitForSeconds(interval);
        InventoryManager.Instance.ItemDescription.SetActive(true);
    }

}
