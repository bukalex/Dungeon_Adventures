using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject slotPrefab;
    public Color selectedSlot, nonSelectedSlot;
    public Image image;

    private void Awake()
    {
        //unselectSlot();
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

}
