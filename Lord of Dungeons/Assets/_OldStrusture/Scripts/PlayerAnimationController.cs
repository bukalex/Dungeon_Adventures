using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    //public GameObject Coin;
    private Animator animator;
    private KeyCode verticalKeyCode = KeyCode.S;
    private KeyCode horizontalKeyCode = KeyCode.S;
    private bool interrupt = false;
    private PolygonCollider2D[] polygonColliders;
    private GeneralCharacterController characterController;

    void Start()
    {
        animator = GetComponent<Animator>();
        polygonColliders = transform.parent.GetComponents<PolygonCollider2D>();
        characterController = transform.parent.GetComponent<GeneralCharacterController>();

        transform.localPosition = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if (characterController.isAlive())
        {
            //Change direction
            if (Input.GetKeyDown(KeyCode.A))
            {
                DisablePolygonColliders(2);

                horizontalKeyCode = KeyCode.A;
                animator.SetTrigger("left");
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                DisablePolygonColliders(3);

                horizontalKeyCode = KeyCode.D;
                animator.SetTrigger("right");
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                DisablePolygonColliders(1);

                verticalKeyCode = KeyCode.W;
                animator.SetTrigger("back");
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                DisablePolygonColliders(0);

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
            if (Input.GetMouseButton(0) && !interrupt)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
                animator.SetTrigger("attack1");
            }

            //Attack 2
            if (Input.GetMouseButton(1) && !interrupt)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
                animator.SetTrigger("attack2");
            }

            //Stop attacking
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || interrupt)
            {
                animator.ResetTrigger("attack1");
                animator.ResetTrigger("attack2");
            }
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
        StartCoroutine(Interrupt());

        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);

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
