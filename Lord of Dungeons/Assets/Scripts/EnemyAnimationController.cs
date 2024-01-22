using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public enum DirectionName { FRONT, BACK, LEFT, RIGHT }

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        transform.localPosition = new Vector3(0, 0, 0);
    }

    //Call this method when change movement direction
    public void changeDirection(DirectionName direction)
    {
        switch (direction)
        {
            case DirectionName.FRONT:
                animator.SetTrigger("front");
                break;

            case DirectionName.BACK:
                animator.SetTrigger("back");
                break;

            case DirectionName.LEFT:
                animator.SetTrigger("side");
                spriteRenderer.flipX = false;
                break;

            case DirectionName.RIGHT:
                animator.SetTrigger("side");
                spriteRenderer.flipX = true;
                break;
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
    }

    public void Run()
    {
        animator.SetBool("isRunning", true);
    }

    public void Attack()
    {
        Stop();
        animator.SetTrigger("attack");
    }

    public void Defend()
    {
        Stop();
        animator.SetTrigger("defend");
    }

    public void Confuse()
    {
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
        Stop();
        animator.SetTrigger("isDead");
    }
}
