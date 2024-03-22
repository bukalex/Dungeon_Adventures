using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : Timer, IInteractable, ITrainable
{
    [SerializeField]
    private NPCParameters npcParameters;

    [SerializeField]
    private string dialogWindowTag;
    private GameObject dialogWindow;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private GameObject interactIconPrefab;
    private GameObject interactIcon;

    private Transform arrow;
    private Transform arrowTarget;

    private DirectionName directionName = DirectionName.FRONT;
    private bool isTraining = true;
    private bool wasMet = false;

    public enum DirectionName { FRONT, BACK, LEFT, RIGHT }

    void Awake()
    {
        if (npcParameters.type != NPCParameters.NPCType.TELEPORT)
        {
            animator.runtimeAnimatorController = npcParameters.animController;
        }
    }

    void Start()
    {
        switch (npcParameters.type)
        {
            case NPCParameters.NPCType.TRADER:
                dialogWindow = UIManager.Instance.traderWindow;
                if (TrainingManager.Instance != null)
                {
                    dialogWindow.GetComponent<NPCReset>().HideItems(new List<string> { "Small Health Potion" });
                    arrowTarget = TrainingManager.Instance.traiderHouse;
                }
                else
                {
                    dialogWindow.GetComponent<NPCReset>().ShowItems();
                }
                break;
            case NPCParameters.NPCType.WIZARD:
                dialogWindow = UIManager.Instance.wizardWindow;
                if (TrainingManager.Instance != null)
                {
                    dialogWindow.GetComponent<NPCReset>().HideItems(new List<string> { "Fans and Knifes" });
                    arrowTarget = TrainingManager.Instance.wizardHouse;
                }
                else
                {
                    dialogWindow.GetComponent<NPCReset>().ShowItems();
                }
                break;
            case NPCParameters.NPCType.TELEPORT:
                dialogWindow = UIManager.Instance.teleportWindow;
                arrowTarget = transform;
                break;
            case NPCParameters.NPCType.BANKER:
                dialogWindow = UIManager.Instance.bankerWindow;
                if (TrainingManager.Instance != null)
                {
                    arrowTarget = TrainingManager.Instance.bankerHouse;
                }
                break;
            case NPCParameters.NPCType.BLACKSMITH:
                dialogWindow = UIManager.Instance.blacksmithWindow;
                if (TrainingManager.Instance != null)
                {
                    arrowTarget = TrainingManager.Instance.blacksmithHouse;
                }
                break;
        }
        interactIcon = Instantiate(interactIconPrefab, Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2), Quaternion.identity, GameObject.FindGameObjectWithTag("MainCanvas").transform);
        interactIcon.transform.SetSiblingIndex(0);
        interactIcon.SetActive(false);

        arrow = Instantiate(UIManager.Instance.arrowPrefab, UIManager.Instance.transform).transform;
        arrow.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (arrowTarget != null) UIManager.Instance.UpdateArrowDirection(arrow, arrowTarget);
    }

    public void InteractWithPlayer(bool isActive)
    {
        if (npcParameters.type != NPCParameters.NPCType.TELEPORT)
        {
            ChangeDirection(Vector2.SignedAngle(Vector3.right, npcParameters.playerData.position - transform.position));
            Greeting();
        }

        if (dialogWindow != null)
        {
            dialogWindow.SetActive(isActive);
            if (!wasMet) wasMet = true;
        }

        UIManager.Instance.npcWindowActive = isActive;
    }

    public float GetColliderRadius()
    {
        return npcParameters.colliderRadius;
    }

    public void ShowButton()
    {
        if (interactIcon != null)
        {
            interactIcon.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
            interactIcon.GetComponentInChildren<TMP_Text>().text = UIManager.Instance.textKeys[15].text;
            interactIcon.SetActive(true);
        } 
    }

    public void HideButton()
    {
        if (interactIcon != null) interactIcon.SetActive(false);
    }

    public IEnumerator StartTraining()
    {
        TrainingManager.Instance.uiBlocked = false;
        TrainingManager.Instance.movementBlocked = false;
        arrow.gameObject.SetActive(true);
        switch (npcParameters.type)
        {
            case NPCParameters.NPCType.BANKER:
                //Dialog
                TrainingManager.Instance.dialogPanel.SetActive(true);
                TrainingManager.Instance.nameText.text = "";
                TrainingManager.Instance.textFieldObject.text = "";
                StartCoroutine(TrainingManager.Instance.StartTyping("Player:", TrainingManager.Instance.nameText));
                yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                StartCoroutine(TrainingManager.Instance.StartTyping("Today I have to deposit these coins on my account.", TrainingManager.Instance.textFieldObject));
                yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                yield return new WaitForSeconds(2.0f);
                TrainingManager.Instance.dialogPanel.SetActive(false);

                //Go to
                wasMet = false;
                TrainingManager.Instance.AddTask("Come to the Banker");
                while (TrainingManager.Instance.HasUndoneTasks())
                {
                    TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = wasMet;
                    yield return null;
                }

                TrainingManager.Instance.uiBlocked = true;
                TrainingManager.Instance.movementBlocked = true;
                StartCoroutine(TrainingManager.Instance.RemoveTasks());
                yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

                //Deposit, convert, expand
                TrainingManager.Instance.AddTask("Deposit the coins");
                TrainingManager.Instance.AddTask("Convert some coins");
                TrainingManager.Instance.AddTask("Buy additional slots to expand your vault");
                while (TrainingManager.Instance.HasUndoneTasks())
                {
                    if ((DataManager.Instance.playerData.resources[Item.CoinType.GoldenCoin] == 0 ||
                        DataManager.Instance.playerData.resources[Item.CoinType.SilverCoin] == 0 ||
                        DataManager.Instance.playerData.resources[Item.CoinType.CopperCoin] == 0) &&
                        !InventoryManager.Instance.HasCoins())
                    {
                        TrainingManager.Instance.dialogPanel.SetActive(true);
                        TrainingManager.Instance.nameText.text = "";
                        TrainingManager.Instance.textFieldObject.text = "";
                        StartCoroutine(TrainingManager.Instance.StartTyping("Banker:", TrainingManager.Instance.nameText));
                        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                        StartCoroutine(TrainingManager.Instance.StartTyping("Not enough coins? I`ll give you some, but it`s the last time.", TrainingManager.Instance.textFieldObject));
                        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                        yield return new WaitForSeconds(2.0f);
                        TrainingManager.Instance.dialogPanel.SetActive(false);
                        TrainingManager.Instance.AddCoins(5);

                        TrainingManager.Instance.wasDeposited = false;
                        TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = false;
                    }

                    TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = TrainingManager.Instance.wasDeposited;
                    TrainingManager.Instance.taskList.GetChild(1).GetComponent<Toggle>().isOn = TrainingManager.Instance.wasConverted;
                    TrainingManager.Instance.taskList.GetChild(2).GetComponent<Toggle>().isOn = TrainingManager.Instance.vaultWasExpanded;
                    yield return null;
                }

                TrainingManager.Instance.uiBlocked = false;
                TrainingManager.Instance.movementBlocked = false;
                StartCoroutine(TrainingManager.Instance.RemoveTasks());
                yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);
                break;

            case NPCParameters.NPCType.TRADER:
                //Dialog
                TrainingManager.Instance.dialogPanel.SetActive(true);
                TrainingManager.Instance.nameText.text = "";
                TrainingManager.Instance.textFieldObject.text = "";
                StartCoroutine(TrainingManager.Instance.StartTyping("Player:", TrainingManager.Instance.nameText));
                yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                StartCoroutine(TrainingManager.Instance.StartTyping("I haven`t seen the Trader for awhile. Maybe he has something for me.", TrainingManager.Instance.textFieldObject));
                yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                yield return new WaitForSeconds(2.0f);
                TrainingManager.Instance.dialogPanel.SetActive(false);

                //Go to
                wasMet = false;
                TrainingManager.Instance.AddTask("Come to the Trader");
                while (TrainingManager.Instance.HasUndoneTasks())
                {
                    TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = wasMet;
                    yield return null;
                }

                TrainingManager.Instance.uiBlocked = true;
                TrainingManager.Instance.movementBlocked = true;
                StartCoroutine(TrainingManager.Instance.RemoveTasks());
                yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

                //Buy, sell
                TrainingManager.Instance.itemPurchased = false;
                TrainingManager.Instance.itemSold = false;
                TrainingManager.Instance.AddTask("Choose Small Health Potion and buy it");
                TrainingManager.Instance.AddTask("Put any item to the sell bar and sell it");
                while (TrainingManager.Instance.HasUndoneTasks())
                {
                    if (DataManager.Instance.playerData.resources[Item.CoinType.CopperCoin] < 50 && !TrainingManager.Instance.itemPurchased)
                    {
                        TrainingManager.Instance.dialogPanel.SetActive(true);
                        TrainingManager.Instance.nameText.text = "";
                        TrainingManager.Instance.textFieldObject.text = "";
                        StartCoroutine(TrainingManager.Instance.StartTyping("Trader:", TrainingManager.Instance.nameText));
                        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                        StartCoroutine(TrainingManager.Instance.StartTyping("Not enough coins? I`ll give you some, but it`s the last time.", TrainingManager.Instance.textFieldObject));
                        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                        yield return new WaitForSeconds(2.0f);
                        TrainingManager.Instance.dialogPanel.SetActive(false);
                        DataManager.Instance.playerData.resources[Item.CoinType.CopperCoin] += 50;
                    }

                    TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = TrainingManager.Instance.itemPurchased;
                    TrainingManager.Instance.taskList.GetChild(1).GetComponent<Toggle>().isOn = TrainingManager.Instance.itemSold;
                    yield return null;
                }

                TrainingManager.Instance.uiBlocked = false;
                TrainingManager.Instance.movementBlocked = false;
                StartCoroutine(TrainingManager.Instance.RemoveTasks());
                yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

                break;

            case NPCParameters.NPCType.BLACKSMITH:
                //Dialog
                TrainingManager.Instance.dialogPanel.SetActive(true);
                TrainingManager.Instance.nameText.text = "";
                TrainingManager.Instance.textFieldObject.text = "";
                StartCoroutine(TrainingManager.Instance.StartTyping("Player:", TrainingManager.Instance.nameText));
                yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                StartCoroutine(TrainingManager.Instance.StartTyping("I need a new sword. Let`s see what our Blacksmith can do.", TrainingManager.Instance.textFieldObject));
                yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                yield return new WaitForSeconds(2.0f);
                TrainingManager.Instance.dialogPanel.SetActive(false);

                //Go to
                wasMet = false;
                TrainingManager.Instance.AddTask("Come to the Blacksmith");
                while (TrainingManager.Instance.HasUndoneTasks())
                {
                    TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = wasMet;
                    yield return null;
                }

                TrainingManager.Instance.uiBlocked = true;
                TrainingManager.Instance.movementBlocked = true;
                StartCoroutine(TrainingManager.Instance.RemoveTasks());
                yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

                //Create Iron Sword
                TrainingManager.Instance.AddItem(1, 1);
                TrainingManager.Instance.AddItem(2, 3);
                TrainingManager.Instance.AddItem(3, 1);
                //TrainingManager.Instance.AddTask("Create Iron Sword");
                //while (TrainingManager.Instance.HasUndoneTasks())
                //{
                //    TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = wasMet;
                //    yield return null;
                //}
                //
                TrainingManager.Instance.uiBlocked = false;
                TrainingManager.Instance.movementBlocked = false;
                //StartCoroutine(TrainingManager.Instance.RemoveTasks());
                //yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

                //Equip Iron Sword
                TrainingManager.Instance.AddTask("Put Iron Sword to the weapon slot in the inventory to equip it");
                while (TrainingManager.Instance.HasUndoneTasks())
                {
                    TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = InventoryManager.Instance.swordSlot.GetComponentInChildren<InventoryItem>() != null;
                    yield return null;
                }
                
                StartCoroutine(TrainingManager.Instance.RemoveTasks());
                yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

                break;

            case NPCParameters.NPCType.WIZARD:
                //Dialog
                TrainingManager.Instance.dialogPanel.SetActive(true);
                TrainingManager.Instance.nameText.text = "";
                TrainingManager.Instance.textFieldObject.text = "";
                StartCoroutine(TrainingManager.Instance.StartTyping("Player:", TrainingManager.Instance.nameText));
                yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                StartCoroutine(TrainingManager.Instance.StartTyping("Yesterday the Wizard said he wanted to show me something.", TrainingManager.Instance.textFieldObject));
                yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                yield return new WaitForSeconds(2.0f);
                TrainingManager.Instance.dialogPanel.SetActive(false);

                //Go to
                wasMet = false;
                TrainingManager.Instance.AddTask("Come to the Wizard");
                while (TrainingManager.Instance.HasUndoneTasks())
                {
                    TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = wasMet;
                    yield return null;
                }

                TrainingManager.Instance.uiBlocked = true;
                TrainingManager.Instance.movementBlocked = true;
                StartCoroutine(TrainingManager.Instance.RemoveTasks());
                yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

                //Buy, exchange
                TrainingManager.Instance.itemPurchased = false;
                TrainingManager.Instance.itemSold = false;
                TrainingManager.Instance.AddTask("Choose Encyclopedia of shields and buy it");
                TrainingManager.Instance.AddTask("Put any artifact to the exchange bar and exchange it for a new ability");
                TrainingManager.Instance.AddItem(4, 1);
                while (TrainingManager.Instance.HasUndoneTasks())
                {
                    if (DataManager.Instance.playerData.resources[Item.CoinType.GoldenCoin] < 5 && !TrainingManager.Instance.itemPurchased)
                    {
                        TrainingManager.Instance.dialogPanel.SetActive(true);
                        TrainingManager.Instance.nameText.text = "";
                        TrainingManager.Instance.textFieldObject.text = "";
                        StartCoroutine(TrainingManager.Instance.StartTyping("Wizard:", TrainingManager.Instance.nameText));
                        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                        StartCoroutine(TrainingManager.Instance.StartTyping("Not enough coins? I`ll give you some, but it`s the last time.", TrainingManager.Instance.textFieldObject));
                        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
                        yield return new WaitForSeconds(2.0f);
                        TrainingManager.Instance.dialogPanel.SetActive(false);
                        DataManager.Instance.playerData.resources[Item.CoinType.GoldenCoin] += 5;
                    }

                    TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = TrainingManager.Instance.itemPurchased;
                    TrainingManager.Instance.taskList.GetChild(1).GetComponent<Toggle>().isOn = TrainingManager.Instance.itemSold;
                    yield return null;
                }

                TrainingManager.Instance.uiBlocked = false;
                TrainingManager.Instance.movementBlocked = false;
                StartCoroutine(TrainingManager.Instance.RemoveTasks());
                yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

                break;

            case NPCParameters.NPCType.TELEPORT:
                break;
        }
        
        arrow.gameObject.SetActive(false);
        isTraining = false;
    }

    public bool IsTraining()
    {
        return isTraining;
    }

    //Animation
    #region
    //Change movement direction
    private void ChangeDirection(float angle)
    {
        if (Mathf.Abs(angle) > 135 && directionName != DirectionName.LEFT)
        {
            directionName = DirectionName.LEFT;
            animator.SetTrigger("side");
            spriteRenderer.flipX = false;
        }
        else if (135 >= angle && angle > 45 && directionName != DirectionName.BACK)
        {
            directionName = DirectionName.BACK;
            animator.SetTrigger("back");
        }
        else if (-135 <= angle && angle < -45 && directionName != DirectionName.FRONT)
        {
            directionName = DirectionName.FRONT;
            animator.SetTrigger("front");
        }
        else if (Mathf.Abs(angle) <= 45 && directionName != DirectionName.RIGHT)
        {
            directionName = DirectionName.RIGHT;
            animator.SetTrigger("side");
            spriteRenderer.flipX = true;
        }
    }

    private void Greeting()
    {
        animator.SetTrigger("greeting");
    }

    private void Walk()
    {
        animator.SetBool("isWalking", true);
    }

    private void Joy()
    {
        animator.SetBool("joy", true);
    }

    private void Stop()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("joy", false);
    }
    #endregion
}
