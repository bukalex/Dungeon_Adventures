using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static BattleManager;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Animator shield;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private CapsuleCollider2D capsuleCollider;

    private Vector3 movementDirection = Vector3.zero;

    private bool alreadyDead = false;
    private bool isTalkingToNPC = false;
    private bool isLooting = false;
    private bool shieldActivated = false;
    private bool isAttacking = false;

    private NPCController activeNPC = null;
    private LootableController activeLootable = null;

    void Awake()
    {
        playerData.position = transform.position;
        animator.runtimeAnimatorController = playerData.animController;
    }

    void Update()
    {
        playerData.position = transform.position;
        playerData.isUsingMana = false;
        playerData.isUsingStamina = false;
        
        if (playerData.IsAlive())
        {
            //Movement
            #region
            movementDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;
            
            //Orientation
            if (DirectionWasChanged())
            {
                ChangeDirection(Vector2.SignedAngle(Vector3.right, movementDirection));
            }

            if (movementDirection.magnitude != 0)
            {
                playerData.attackDirection = movementDirection;

                if (Input.GetKey(KeyCode.LeftShift) && BattleManager.Instance.PlayerPerformShift(playerData))
                {
                    if (!isAttacking)
                    {
                        Run();
                    }
                    body.velocity = movementDirection * playerData.speed * 2;
                }
                else
                {
                    if (!isAttacking)
                    {
                        Walk();
                    }
                    body.velocity = movementDirection * playerData.speed;
                }
            }
            else
            {
                Stop();
                body.velocity = Vector3.zero;
            }
            #endregion

            //Attacks
            #region
            //Left Mouse Button
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (BattleManager.Instance.PlayerPerformLMB(playerData))
                {
                    body.velocity = movementDirection * playerData.speed * 0.5f;
                    AttackWithLMB();
                    isAttacking = true;
                }
            }

            //Right Mouse Button
            //if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
            //{
            //    attackButton = PlayerData.AttackButton.RMB;
            //
            //    if (playerData.AffordAttack(attackButton))
            //    {
            //        ActivateShield(true);
            //
            //        shieldActivated = true;
            //        playerData.defense *= 5.0f;
            //        playerData.specialDefense *= 5.0f;
            //    }
            //}
            //
            //if (shieldActivated)
            //{
            //    attackButton = PlayerData.AttackButton.RMB;
            //
            //    if ((Input.GetMouseButtonUp(1) || !playerData.AffordAttack(attackButton, true)) && !EventSystem.current.IsPointerOverGameObject())
            //    {
            //        attackButton = PlayerData.AttackButton.NONE;
            //
            //        ActivateShield(false);
            //
            //        shieldActivated = false;
            //        playerData.defense /= 5.0f;
            //        playerData.specialDefense /= 5.0f;
            //    }
            //}
            #endregion

            //NPC interaction
            #region
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (activeNPC == null)
                {
                    isTalkingToNPC = false;
                }
                isTalkingToNPC = !isTalkingToNPC;

                List<NPCController> npcs = DetectTargets<NPCController>(playerData.npcDetectionRadius + playerData.colliderRadius, false);

                if (npcs.Count > 0)
                {
                    activeNPC = npcs[0];
                    activeNPC.InteractWithPlayer(isTalkingToNPC);
                }
            }

            if (activeNPC != null)
            {
                if ((activeNPC.transform.position - transform.position).magnitude - playerData.colliderRadius - activeNPC.GetColliderRadius() > playerData.npcDetectionRadius)
                {
                    activeNPC.InteractWithPlayer(false);
                    activeNPC = null;
                }
            }
            #endregion

            //Chest Interactions
            #region
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (activeLootable == null)
                {
                    isLooting = false;
                }
                isLooting = !isLooting;

                List<LootableController> lootables = DetectTargets<LootableController>(playerData.lootableDetectionRadius + playerData.colliderRadius, false);

                if (lootables.Count > 0)
                {
                    activeLootable = lootables[0];
                    activeLootable.InteractWithPlayer(isLooting);
                }
            }

            if (activeLootable != null)
            {
                if ((activeLootable.transform.position - transform.position).magnitude - playerData.colliderRadius - activeLootable.GetColliderRadius() > playerData.lootableDetectionRadius)
                {
                    activeLootable.InteractWithPlayer(false);
                    activeLootable = null;
                }
            }
            #endregion

            //Stats restore
            playerData.RestoreStats();
        }
        else if (!alreadyDead)
        {
            Die();

            capsuleCollider.enabled = false;
            body.velocity = Vector3.zero;

            SetShield(false);

            alreadyDead = true;
        }
    }

    //Detect targets in the 90 degree sector in front of the player
    private List<T> DetectTargets<T>(float range, bool inSector = true)
    {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, range);
        List<T> targets = new List<T>();

        foreach (Collider2D target in possibleTargets)
        {
            if (target.GetComponent<T>() != null)
            {
                Vector2 targetDirection = target.transform.position - transform.position;

                if (!inSector)
                {
                    targets.Add(target.GetComponent<T>());
                }
                else if (Vector2.Angle(playerData.attackDirection, targetDirection) <= 45)
                {
                    targets.Add(target.GetComponent<T>());
                }
            }
        }

        return targets;
    }

    private bool DirectionWasChanged()
    {
        return Input.GetKeyDown(KeyCode.W) || 
            Input.GetKeyDown(KeyCode.A) || 
            Input.GetKeyDown(KeyCode.S) || 
            Input.GetKeyDown(KeyCode.D) || 
            Input.GetKeyUp(KeyCode.W) || 
            Input.GetKeyUp(KeyCode.A) || 
            Input.GetKeyUp(KeyCode.S) || 
            Input.GetKeyUp(KeyCode.D);
    }

    //Animation
    #region
    //Activate/deactivate shield
    private void SetShield(bool isActive)
    {
        shield.SetBool("shieldActivated", isActive);
    }

    //Change movement direction
    private void ChangeDirection(float angle)
    {
        if (Mathf.Abs(angle) > 135)
        {
            animator.SetTrigger("left");
        }
        else if (135 >= angle && angle > 45)
        {
            animator.SetTrigger("back");
        }
        else if (-135 <= angle && angle < -45)
        {
            animator.SetTrigger("front");
        }
        else if (Mathf.Abs(angle) <= 45)
        {
            animator.SetTrigger("right");
        }
    }

    //Stop walking and running
    private void Stop()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
    }

    //Start walking
    private void Walk()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
    }

    //Start running
    private void Run()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", true);
    }

    //Attack 1
    private void AttackWithLMB()
    {
        Stop();
        animator.SetTrigger("attack1");
    }

    //Die
    private void Die()
    {
        Stop();
        animator.SetTrigger("isDead");
    }
    #endregion
}
