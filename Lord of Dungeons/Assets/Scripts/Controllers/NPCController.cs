using TMPro;
using UnityEngine;

public class NPCController : Timer, IInteractable
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

    private DirectionName directionName = DirectionName.FRONT;

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
                break;
            case NPCParameters.NPCType.WIZARD:
                dialogWindow = UIManager.Instance.wizardWindow;
                break;
            case NPCParameters.NPCType.TELEPORT:
                dialogWindow = UIManager.Instance.teleportWindow;
                break;
            case NPCParameters.NPCType.BANKER:
                dialogWindow = UIManager.Instance.bankerWindow;
                break;
            case NPCParameters.NPCType.BLACKSMITH:
                dialogWindow = UIManager.Instance.blacksmithWindow;
                break;
        }
        interactIcon = Instantiate(interactIconPrefab, Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2), Quaternion.identity, GameObject.FindGameObjectWithTag("MainCanvas").transform);
        interactIcon.transform.SetSiblingIndex(0);
        interactIcon.SetActive(false);
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
