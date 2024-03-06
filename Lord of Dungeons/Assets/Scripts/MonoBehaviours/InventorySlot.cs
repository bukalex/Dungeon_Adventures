using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IDropHandler /*IPointerEnterHandler, IPointerExitHandler*/
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
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    GameObject slot = eventData.pointerEnter;
    //    InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
    //    if (itemInSlot != null)
    //    {
    //        StartCoroutine(onItemDescribe(0.75f));
    //        InventoryManager.Instance.ItemDescription.transform.GetChild(0).GetComponent<TMP_Text>().text = itemInSlot.item.name;
    //        InventoryManager.Instance.ItemDescription.transform.GetChild(1).GetComponent<TMP_Text>().text = itemInSlot.item.description;
    //    }
    //}
    //
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    InventoryManager.Instance.ItemDescription.SetActive(false);
    //}
}
