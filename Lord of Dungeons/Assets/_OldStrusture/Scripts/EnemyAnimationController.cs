using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool interrupt = false;
    private PolygonCollider2D[] polygonColliders;
    private DirectionName direction = DirectionName.FRONT;

    private enum DirectionName { FRONT, BACK, LEFT, RIGHT }

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonColliders = transform.parent.GetComponents<PolygonCollider2D>();

        transform.localPosition = new Vector3(0, 0, 0);
    }

    //Call this method when change movement direction
    public void ChangeDirection(float angle)
    {
        if (Mathf.Abs(angle) > 135 && direction != DirectionName.LEFT)
        {
            DisablePolygonColliders(2);

            direction = DirectionName.LEFT;
            animator.SetTrigger("side");
            spriteRenderer.flipX = false;
        }
        else if (135 >= angle && angle > 45 && direction != DirectionName.BACK)
        {
            DisablePolygonColliders(1);

            direction = DirectionName.BACK;
            animator.SetTrigger("back");
        }
        else if (-135 <= angle && angle < -45 && direction != DirectionName.FRONT)
        {
            DisablePolygonColliders(0);

            direction = DirectionName.FRONT;
            animator.SetTrigger("front");
        }
        else if (Mathf.Abs(angle) <= 45 && direction != DirectionName.RIGHT)
        {
            DisablePolygonColliders(3);

            direction = DirectionName.RIGHT;
            animator.SetTrigger("side");
            spriteRenderer.flipX = true;
        }
    }

    //This method stops walking, running, and confusion
    public void Stop()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isConfused", false);
    }

    public void Walk()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
    }

    public void Run()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", true);
    }

    public void Attack()
    {
        if (!interrupt)
        {
            Stop();
            animator.SetTrigger("attack");
        }
    }

    public void Defend()
    {
        Stop();
        animator.SetTrigger("defend");
    }

    public void Confuse()
    {
        StartCoroutine(Interrupt());
        Stop();
        animator.SetBool("isConfused", true);
    }

    public void Hurt()
    {
        Stop();
        animator.SetTrigger("isHurt");
    }

    public void Die()
    {
        StartCoroutine(Interrupt());
        Stop();
        animator.SetTrigger("isDead");
    }

    IEnumerator Interrupt()
    {
        interrupt = true;
        yield return new WaitForSeconds(1.0f);
        interrupt = false;
    }

    private void DisablePolygonColliders(int exceptionIndex)
    {
        foreach (PolygonCollider2D polygonCollider in polygonColliders)
        {
            polygonCollider.enabled = false;
        }

        polygonColliders[exceptionIndex].enabled = true;
    }
}