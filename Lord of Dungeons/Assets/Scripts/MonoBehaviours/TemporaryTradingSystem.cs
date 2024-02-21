using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TemporaryTradingSystem : MonoBehaviour
{
    public InventorySlot sellSlot;
    public PlayerData playerData;
    public Item[] items2Pickup;
    public InventoryItem itemInStore;
    public TMP_Text priceDisplay;
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public TMP_Text[] itemPrice;
    private int[] coinsFromSell;

    public GraphicRaycaster graphicRaycaster;
    public EventSystem eventSystem;
    private PointerEventData eventData;

    private void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            eventData = new PointerEventData(eventSystem);
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.tag == "ItemHolder")
                {
                    if (itemInStore != null)
                    {
                        itemInStore.GetComponentInParent<InventorySlot>().unselectSlot();
                    }

                    itemInStore = result.gameObject.GetComponentInChildren<InventorySlot>().GetComponentInChildren<InventoryItem>();
                    itemInStore.GetComponentInParent<InventorySlot>().selectSlot();
                    itemName.text = itemInStore.item.name;
                    itemDescription.text = itemInStore.item.description.Replace("\\n", "\n");
                    break;
                }
            }
        }
    }

    public void Purchase()
    {
        if (itemInStore != null && 
            itemInStore.item.GoldenCoin <= playerData.resources[Item.CoinType.GoldenCoin] &&
            itemInStore.item.SilverCoin <= playerData.resources[Item.CoinType.SilverCoin] &&
            itemInStore.item.CopperCoin <= playerData.resources[Item.CoinType.CopperCoin])
        {
            playerData.resources[Item.CoinType.GoldenCoin] -= itemInStore.item.GoldenCoin;
            playerData.resources[Item.CoinType.SilverCoin] -= itemInStore.item.SilverCoin;
            playerData.resources[Item.CoinType.CopperCoin] -= itemInStore.item.CopperCoin;

            InventoryManager.Instance.AddItem(itemInStore.item, InventoryManager.Instance.toolBar, InventoryManager.Instance.internalInventorySlots);
        }
    }

    public void PickUpItem(int id)
    {

        //if (items2Pickup[id].price <= playerData.resources[Item.MaterialType.Coin])
        //{
        //   //InventoryManager.Instance.AddItem(items2Pickup[id]);
        //   //
        //   //playerData.resources[Item.MaterialType.Coin] -= items2Pickup[id].price;
        //}
    }

    public void sellItems()
    {
        coinsFromSell = new int[] { 0, 0, 0 };

        foreach (InventorySlot sellSlot in InventoryManager.Instance.sellSlots)
        {
            InventoryItem inventoryItem = sellSlot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null)
            {
                coinsFromSell[0] += inventoryItem.item.GoldenCoins * inventoryItem.count;
                coinsFromSell[1] += inventoryItem.item.SilverCoins * inventoryItem.count;
                coinsFromSell[2] += inventoryItem.item.CopperCoins * inventoryItem.count;

                Destroy(inventoryItem.gameObject);
            }
        }

        playerData.resources[Item.CoinType.GoldenCoin] += coinsFromSell[0];
        playerData.resources[Item.CoinType.SilverCoin] += coinsFromSell[1];
        playerData.resources[Item.CoinType.CopperCoin] += coinsFromSell[2];

        priceDisplay.text = "";
        StartCoroutine(ChangeLabel());
    }

    private IEnumerator ChangeLabel()
    {
        priceDisplay.text = "";
        for (int i = 0; i < priceDisplay.transform.childCount; i++)
        {
            priceDisplay.transform.GetChild(i).gameObject.SetActive(true);
            if (i % 2 == 1) priceDisplay.GetComponentsInChildren<TMP_Text>()[1 + i / 2].text = coinsFromSell[i / 2].ToString();
        }

        yield return new WaitForSeconds(5.0f);

        priceDisplay.text = "Place items that you want to sell";
        for (int i = 0; i < priceDisplay.transform.childCount; i++)
        {
            priceDisplay.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private bool isSlotEmpty()
    {

        InventorySlot slot = sellSlot;
        InventoryItem itemInSlot = sellSlot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
            return false;

        Debug.Log("Slot is empty");
        return true;
    }
}

