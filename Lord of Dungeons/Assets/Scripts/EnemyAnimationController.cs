using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    public GameObject Coin;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool interrupt = false;
    private DirectionName direction = DirectionName.FRONT;

    private enum DirectionName { FRONT, BACK, LEFT, RIGHT }

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        transform.localPosition = new Vector3(0, 0, 0);
    }

    //Call this method when change movement direction
    public void ChangeDirection(float angle)
    {
        if (Mathf.Abs(angle) > 135 && direction != DirectionName.LEFT)
        {
            direction = DirectionName.LEFT;
            animator.SetTrigger("side");
            spriteRenderer.flipX = false;
        }
        else if (135 >= angle && angle > 45 && direction != DirectionName.BACK)
        {
            direction = DirectionName.BACK;
            animator.SetTrigger("back");
        }
        else if (-135 <= angle && angle < -45 && direction != DirectionName.FRONT)
        {
            direction = DirectionName.FRONT;
            animator.SetTrigger("front");
        }
        else if (Mathf.Abs(angle) <= 45 && direction != DirectionName.RIGHT)
        {
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
        Instantiate(Coin, transform.position, Quaternion.identity);
    }

    IEnumerator Interrupt()
    {
        interrupt = true;
        yield return new WaitForSeconds(1.0f);
        interrupt = false;
    }
}
