using System.Collections;
using System.Collections.Generic;
using TMPro;
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
                arrowTarget = TrainingManager.Instance.traiderHouse;
                break;
            case NPCParameters.NPCType.WIZARD:
                dialogWindow = UIManager.Instance.wizardWindow;
                arrowTarget = TrainingManager.Instance.wizardHouse;
                break;
            case NPCParameters.NPCType.TELEPORT:
                dialogWindow = UIManager.Instance.teleportWindow;
                arrowTarget = transform;
                break;
            case NPCParameters.NPCType.BANKER:
                dialogWindow = UIManager.Instance.bankerWindow;
                arrowTarget = TrainingManager.Instance.bankerHouse;
                break;
            case NPCParameters.NPCType.BLACKSMITH:
                dialogWindow = UIManager.Instance.blacksmithWindow;
                arrowTarget = TrainingManager.Instance.blacksmithHouse;
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
                TrainingManager.Instance.AddTask("Come to the Banker");
                while (TrainingManager.Instance.HasUndoneTasks())
                {
                    if (Input.GetKeyDown(UIManager.Instance.keyCodes[13]))
                    {
                        TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = true;
                        TrainingManager.Instance.uiBlocked = true;
                    }
                    yield return null;
                }

                StartCoroutine(TrainingManager.Instance.RemoveTasks());
                yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);
                break;

            case NPCParameters.NPCType.TRADER:
                break;

            case NPCParameters.NPCType.WIZARD:
                break;

            case NPCParameters.NPCType.BLACKSMITH:
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
