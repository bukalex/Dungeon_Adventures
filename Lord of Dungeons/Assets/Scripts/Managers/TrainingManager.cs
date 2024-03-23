using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.UIElements;

public class TrainingManager : MonoBehaviour
{
    public bool movementBlocked = true;
    public bool attacksBlocked = true;
    public bool uiBlocked = true;
    public bool itemUsageBlocked = true;
    public bool isTyping = false;
    public bool isRemovingTasks = false;
    public bool isInside = false;
    public bool canGoOut = false;

    public bool itemWasDraggedAndMoved = false;
    public bool itemWasClickedAndMoved = false;
    public bool wasDeposited = false;
    public bool wasConverted = false;
    public bool vaultWasExpanded = false;
    public bool itemPurchased = false;
    public bool itemSold = false;

    public GameObject taskPrefab;
    [SerializeField] private List<Item> coins;
    [SerializeField] private List<Item> tutorialItems;
    [SerializeField] private List<MonoBehaviour> trainables;

    public Transform taskList;

    public Transform bankerHouseOutside, traiderHouseOutside, blacksmithHouseOutside, wizardHouseOutside;
    public Transform bankerHouseInside, traiderHouseInside, blacksmithHouseInside, wizardHouseInside;

    public GameObject dialogPanel;
    public TMP_Text textFieldObject, nameText;

    public static TrainingManager Instance { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StartCoroutine(StartTraining());
        }
    }

    private IEnumerator StartTraining()
    {
        yield return new WaitForSeconds(1.0f);
        foreach (ITrainable trainable in trainables)
        {
            StartCoroutine(trainable.StartTraining());
            yield return new WaitWhile(() => trainable.IsTraining());
            yield return new WaitForSeconds(1.0f);
        }

        movementBlocked = false;
        attacksBlocked = false;
        uiBlocked = false;
        itemUsageBlocked = false;
    }

    public IEnumerator StartTyping(string text, TMP_Text textField)
    {
        isTyping = true;
        textField.text = "";
        foreach (char symbol in text)
        {
            textField.text += symbol;
            yield return new WaitForSeconds(0.025f);
        }
        isTyping = false;
    }

    public bool HasUndoneTasks()
    {
        foreach (Toggle task in taskList.GetComponentsInChildren<Toggle>()) if (!task.isOn) return true;
        return false;
    }

    public void AddTask(string description)
    {
        TMP_Text task = Instantiate(taskPrefab, taskList).GetComponentInChildren<TMP_Text>();
        task.text = description;
    }

    public IEnumerator RemoveTasks(bool closeUI = false)
    {
        isRemovingTasks = true;
        yield return new WaitForSeconds(1.0f);
        if (closeUI)
        {
            UIManager.Instance.SwitchInventory();
        }

        for (int i = taskList.childCount-1; i >= 0; i--)
        {
            for (int j = 0; j < 100; j++)
            {
                taskList.GetChild(i).Translate(new Vector3(20.0f, 0, 0));
                yield return new WaitForEndOfFrame();
            }
            Destroy(taskList.GetChild(i).gameObject);
        }
        isRemovingTasks = false;
    }

    public void AddCoins(int amount)
    {
        foreach (Item coin in coins)
        {
            for (int i = 0; i < amount; i++)
            {
                InventoryManager.Instance.AddItem(coin, InventoryManager.Instance.internalInventorySlots, InventoryManager.Instance.toolBar);
            }
        }
    }

    public void AddItem(int index, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            InventoryManager.Instance.AddItem(tutorialItems[index], InventoryManager.Instance.internalInventorySlots, InventoryManager.Instance.toolBar);
        }
    }
}
