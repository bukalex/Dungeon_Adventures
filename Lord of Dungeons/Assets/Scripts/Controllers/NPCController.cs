using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField]
    private NPCParameters npcParameters;

    [SerializeField]
    private GameObject dialogWindow;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private DirectionName directionName = DirectionName.FRONT;
    private int spriteOrder;
    private int enemyOrder;
    private float angle;

    public enum DirectionName { FRONT, BACK, LEFT, RIGHT }

    void Awake()
    {
        if (npcParameters.type != NPCParameters.NPCType.TELEPORT)
        {
            animator.runtimeAnimatorController = npcParameters.animController;
        }
    }

    void Update()
    {
        List<SpriteRenderer> renderers = DetectSprites<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.sortingLayerName.Equals(spriteRenderer.sortingLayerName))
            {
                angle = Vector2.SignedAngle(Vector3.right, renderer.transform.parent.position - transform.position);
                if (135 >= angle && angle > 45)
                {
                    spriteOrder = renderer.sortingOrder;
                    enemyOrder = spriteRenderer.sortingOrder;
                    if (spriteOrder > enemyOrder)
                    {
                        spriteRenderer.sortingOrder = spriteOrder;
                        renderer.sortingOrder = enemyOrder;
                    }
                    else if (spriteOrder == enemyOrder)
                    {
                        spriteRenderer.sortingOrder += 1;
                    }
                }
                else if (-135 <= angle && angle < -45)
                {
                    spriteOrder = renderer.sortingOrder;
                    enemyOrder = spriteRenderer.sortingOrder;
                    if (spriteOrder < enemyOrder)
                    {
                        spriteRenderer.sortingOrder = spriteOrder;
                        renderer.sortingOrder = enemyOrder;
                    }
                    else if (spriteOrder == enemyOrder)
                    {
                        renderer.sortingOrder += 1;
                    }
                }
            }
        }
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

    private List<T> DetectSprites<T>()
    {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, npcParameters.colliderRadius * 2);
        List<T> targets = new List<T>();

        foreach (Collider2D target in possibleTargets)
        {
            if (target.GetComponentInChildren<T>() != null)
            {
                targets.Add(target.GetComponentInChildren<T>());
            }
        }

        return targets;
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
