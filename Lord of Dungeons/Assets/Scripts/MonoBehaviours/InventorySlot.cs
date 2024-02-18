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

        if (transform.childCount == 0 && inventoryItem.InventoryItemPrefab.CompareTag("Item") && slotPrefab.CompareTag("Inventory"))
        {
            inventoryItem.parentAfterDrag = transform;
        }
    }

}
