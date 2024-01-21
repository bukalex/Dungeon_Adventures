using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;

    private KeyCode directionKeyCode = KeyCode.S;

    void Update()
    {
        //Change direction
        if (Input.GetKeyDown(KeyCode.A))
        {
            directionKeyCode = KeyCode.A;
            animator.SetTrigger("left");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            directionKeyCode = KeyCode.D;
            animator.SetTrigger("right");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            directionKeyCode = KeyCode.W;
            animator.SetTrigger("back");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            directionKeyCode = KeyCode.S;
            animator.SetTrigger("front");
        }

        //Start walking
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            animator.SetBool("isWalking", true);
        }

        //Stop walking
        if (Input.GetKeyUp(directionKeyCode))
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

    public void PlayHurt()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetTrigger("isHurt");
    }

    public void PlayDied()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetTrigger("isDead");
    }
}
