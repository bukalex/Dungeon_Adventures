using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class InventorySlot : MonoBehaviour
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
        Debug.Log("2");
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
        //Check if item and slot have the same tag
        #region
        //Inventory slots

        //inventoryItem.parentAfterDrag = transform;
        
        #endregion
    }
}
