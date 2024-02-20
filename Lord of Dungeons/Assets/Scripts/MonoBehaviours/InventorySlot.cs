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
        if (transform.childCount == 0 && 
            (inventoryItem.InventoryItemPrefab.CompareTag("Item") && slotPrefab.CompareTag("Inventory")) ||
            (inventoryItem.InventoryItemPrefab.CompareTag("Gem") && slotPrefab.CompareTag("Gem"))||
            (inventoryItem.InventoryItemPrefab.CompareTag("Gem") && slotPrefab.CompareTag("Inventory"))
            )
        {
            inventoryItem.parentAfterDrag = transform;
        }
        ////Chestplate slot
        //else if (transform.childCount == 0 && inventoryItem.InventoryItemPrefab.CompareTag("Chestplate") == true && slotPrefab.CompareTag("Chestplate") == true)
        //{
        //    inventoryItem.parentAfterDrag = transform;
        //}
        ////Helmet slot
        //else if (transform.childCount == 0 && inventoryItem.itemTag == inventoryItem.item.tag.ToString() && slotPrefab.CompareTag("Helmet") == true)
        //{
        //    inventoryItem.parentAfterDrag = transform;
        //}
        ////Gem slot
        //else if (transform.childCount == 0 && inventoryItem.InventoryItemPrefab.CompareTag("Boots") && slotPrefab.CompareTag("Boots"))
        //{
        //    inventoryItem.parentAfterDrag = transform;
        //}
        ////
        //else if (transform.childCount == 0 && inventoryItem.InventoryItemPrefab.CompareTag("Sword") && slotPrefab.CompareTag("Sword"))
        //{
        //    inventoryItem.parentAfterDrag = transform;
        //}
        //else if (transform.childCount == 0 && inventoryItem.itemTag == inventoryItem.item.tag.ToString() && slotPrefab.CompareTag("Gem") == true)
        //{
        //    inventoryItem.parentAfterDrag = transform;
        //}
        #endregion
    }

}
