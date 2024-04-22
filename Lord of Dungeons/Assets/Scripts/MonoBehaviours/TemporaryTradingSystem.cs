using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class TemporaryTradingSystem : MonoBehaviour
{
    public InventorySlot sellSlot;
    public PlayerData playerData;
    public Item[] items2Pickup;
    public static InventoryItem itemInStore;
    public TMP_Text traderPriceDisplay, wizardPriceDisplay;
    public TMP_Text traderItemName, wizardItemName;
    public TMP_Text traderItemDescription, wizardItemDescription;
    public TMP_Text[] itemPrice;
    private int[] coinsFromSell;
    public float wizardLuck = 100.0f;
    public List<Ability> wizardAbilities;

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
                    if (itemInStore != null && itemInStore.GetComponentInParent<InventorySlot>())
                    {
                        itemInStore.GetComponentInParent<InventorySlot>().unselectSlot();
                    }

                    itemInStore = result.gameObject.GetComponentInChildren<InventorySlot>().GetComponentInChildren<InventoryItem>();
                    itemInStore.GetComponentInParent<InventorySlot>().selectSlot();

                    traderItemName.text = itemInStore.item.name;
                    traderItemDescription.text = itemInStore.item.description.Replace("\\n", "\n");

                    wizardItemName.text = itemInStore.item.name;
                    wizardItemDescription.text = itemInStore.item.description.Replace("\\n", "\n");
                    break;
                }
            }
        }
        if (itemInStore == null || !itemInStore.gameObject.activeInHierarchy)
        {
            traderItemName.text = "Item name";
            traderItemDescription.text = "Description";

            wizardItemName.text = "Item name";
            wizardItemDescription.text = "Description";
        }
    }

    public void Purchase()
    {
        if (itemInStore != null && 
            itemInStore.item.GoldenCoin <= playerData.resources[Item.CoinType.GoldenCoin] &&
            itemInStore.item.SilverCoin <= playerData.resources[Item.CoinType.SilverCoin] &&
            itemInStore.item.CopperCoin <= playerData.resources[Item.CoinType.CopperCoin])
        {
            if (TrainingManager.Instance != null) TrainingManager.Instance.itemPurchased = true;
            playerData.resources[Item.CoinType.GoldenCoin] -= itemInStore.item.GoldenCoin;
            playerData.resources[Item.CoinType.SilverCoin] -= itemInStore.item.SilverCoin;
            playerData.resources[Item.CoinType.CopperCoin] -= itemInStore.item.CopperCoin;

            InventoryManager.Instance.AddItem(itemInStore.item, InventoryManager.Instance.toolBar.ToList(), InventoryManager.Instance.internalInventorySlots.ToList());
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

    public void sellItems(int npcIndex)
    {
        switch (npcIndex)
        {
            case 0:
                coinsFromSell = new int[] { 0, 0, 0 };

                foreach (InventorySlot sellSlot in InventoryManager.Instance.traderSellSlots)
                {
                    InventoryItem inventoryItem = sellSlot.GetComponentInChildren<InventoryItem>();
                    if (inventoryItem != null)
                    {
                        if (TrainingManager.Instance != null) TrainingManager.Instance.itemSold = true;
                        coinsFromSell[0] += inventoryItem.item.GoldenCoins * inventoryItem.count;
                        coinsFromSell[1] += inventoryItem.item.SilverCoins * inventoryItem.count;
                        coinsFromSell[2] += inventoryItem.item.CopperCoins * inventoryItem.count;

                        Destroy(inventoryItem.gameObject);
                    }
                }

                playerData.resources[Item.CoinType.GoldenCoin] += coinsFromSell[0];
                playerData.resources[Item.CoinType.SilverCoin] += coinsFromSell[1];
                playerData.resources[Item.CoinType.CopperCoin] += coinsFromSell[2];

                StartCoroutine(ChangeLabel(npcIndex, "Place items that you want to sell", ""));
                break;

            case 1:
                bool canExchange = false;
                foreach (InventorySlot sellSlot in InventoryManager.Instance.wizardSellSlots)
                {
                    InventoryItem inventoryItem = sellSlot.GetComponentInChildren<InventoryItem>();
                    if (inventoryItem != null)
                    {
                        if (TrainingManager.Instance != null) TrainingManager.Instance.itemSold = true;
                        canExchange = true;
                        wizardLuck += 0.1f;

                        Destroy(inventoryItem.gameObject);
                    }
                }

                if (canExchange)
                {
                    if (wizardAbilities.Count == 0) return;

                    if (Random.Range(0.0f, 100.0f) <= wizardLuck)
                    {
                        Ability ability = wizardAbilities[Random.Range(0, wizardAbilities.Count)];
                        wizardAbilities.Remove(ability);
                        InventoryManager.Instance.AddAbility(ability);
                        wizardLuck = 0;

                        if (wizardAbilities.Count != 0)
                        {
                            StartCoroutine(ChangeLabel(npcIndex, "Place artifacts that you want to exchange", "Congratulations! You got new ability"));
                        }
                        else
                        {
                            StartCoroutine(ChangeLabel(npcIndex, "Now you know everything", "Congratulations! You got new ability"));
                        }
                    }
                    else
                    {
                        StartCoroutine(ChangeLabel(npcIndex, "Your chance to get new ability: " + wizardLuck.ToString("F1") + "%", "You got nothing"));
                    }
                }

                break;
        }
    }

    private IEnumerator ChangeLabel(int npcIndex, string defaultText, string temporaryText)
    {
        switch (npcIndex)
        {
            case 0:
                traderPriceDisplay.text = temporaryText;
                for (int i = 0; i < traderPriceDisplay.transform.childCount; i++)
                {
                    traderPriceDisplay.transform.GetChild(i).gameObject.SetActive(true);
                    if (i % 2 == 1) traderPriceDisplay.GetComponentsInChildren<TMP_Text>()[1 + i / 2].text = coinsFromSell[i / 2].ToString();
                }

                yield return new WaitForSeconds(5.0f);

                traderPriceDisplay.text = defaultText;
                for (int i = 0; i < traderPriceDisplay.transform.childCount; i++)
                {
                    traderPriceDisplay.transform.GetChild(i).gameObject.SetActive(false);
                }
                break;

            case 1:
                wizardPriceDisplay.text = temporaryText;
                yield return new WaitForSeconds(3.0f);
                wizardPriceDisplay.text = defaultText;
                break;
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

