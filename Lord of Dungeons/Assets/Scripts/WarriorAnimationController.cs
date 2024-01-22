using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAnimationController : MonoBehaviour
{
    private Animator animator;
    private KeyCode verticalKeyCode = KeyCode.S;
    private KeyCode horizontalKeyCode = KeyCode.S;

    void Start()
    {
        animator = GetComponent<Animator>();
        transform.localPosition = new Vector3(0, 0, 0);
    }

    void Update()
    {
        //Change direction
        if (Input.GetKeyDown(KeyCode.A))
        {
            horizontalKeyCode = KeyCode.A;
            animator.SetTrigger("left");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            horizontalKeyCode = KeyCode.D;
            animator.SetTrigger("right");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            verticalKeyCode = KeyCode.W;
            animator.SetTrigger("back");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            verticalKeyCode = KeyCode.S;
            animator.SetTrigger("front");
        }

        //Start walking
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            animator.SetBool("isWalking", true);
        }

        //Stop walking
        if (!Input.GetKey(verticalKeyCode) && !Input.GetKey(horizontalKeyCode))
        {
            animator.SetBool("isWalking", false);
        }

        //Start runnign
        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("isRunning", true);
        }

        //Stop runnign
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("isRunning", false);
        }

        //Attack 1
        if (Input.GetMouseButton(0))
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            animator.SetTrigger("attack1");
        }

        //Attack 2
        if (Input.GetMouseButton(1))
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            animator.SetTrigger("attack2");
        }

        //Stop attacking
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            animator.ResetTrigger("attack1");
            animator.ResetTrigger("attack2");
        }
    }

    public void Hurt()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetTrigger("isHurt");
    }

    public void Die()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetTrigger("isDead");
    }
}
