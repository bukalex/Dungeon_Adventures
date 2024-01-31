using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private Vector3 attackDirection = Vector3.down;
    private PlayerData.AttackButton attackButton = PlayerData.AttackButton.NONE;

    private bool isReadyToAttack = true;
    private bool arleadyDead = false;
    private bool isTalkingToNPC = false;
    private bool shieldActivated = false;

    private NPCController activeNPC = null;

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

        if (playerData.isAlive())
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
                attackDirection = movementDirection;

                if (Input.GetKey(KeyCode.LeftShift) && playerData.AffordAttack(PlayerData.AttackButton.SHIFT, true))
                {
                    if (attackButton == PlayerData.AttackButton.NONE)
                    {
                        Run();
                    }
                    body.velocity = movementDirection * playerData.speed * 2;
                }
                else
                {
                    if (attackButton == PlayerData.AttackButton.NONE)
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
            switch (playerData.type)
            {
                case PlayerData.CharacterType.WARRIOR:
                    if (isReadyToAttack)
                    {
                        //Left Mouse Button
                        #region
                        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
                        {
                            attackButton = PlayerData.AttackButton.LMB;

                            if (playerData.AffordAttack(attackButton))
                            {
                                body.velocity = movementDirection * playerData.speed * 0.5f;
                                AttackWithLMB();

                                List<EnemyController> enemies = DetectTargets<EnemyController>(playerData.attacksByRange[attackButton] + playerData.colliderRadius);
                                foreach (EnemyController enemy in enemies)
                                {
                                    if (enemy.isAlive())
                                    {
                                        enemy.DealDamage(PlayerData.AttackType.BASIC, playerData.attacksByDamage[attackButton], playerData.attack);
                                    }
                                }
                            }
                        }
                        if (Input.GetMouseButtonUp(0))
                        {
                            attackButton = PlayerData.AttackButton.NONE;
                        }
                        #endregion

                        //Cooldown
                        if (playerData.attacksByCooldown.ContainsKey(attackButton))
                        {
                            StartCoroutine(Cooldown(playerData.attacksByCooldown[attackButton]));
                        }
                    }

                    //Right Mouse Button
                    #region
                    if (Input.GetMouseButtonDown(1) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        attackButton = PlayerData.AttackButton.RMB;

                        if (playerData.AffordAttack(attackButton))
                        {
                            ActivateShield(true);

                            shieldActivated = true;
                            playerData.defense *= 5.0f;
                            playerData.specialDefense *= 5.0f;
                        }
                    }

                    if (shieldActivated)
                    {
                        attackButton = PlayerData.AttackButton.RMB;

                        if ((Input.GetMouseButtonUp(1) || !playerData.AffordAttack(attackButton, true)) && !EventSystem.current.IsPointerOverGameObject())
                        {
                            attackButton = PlayerData.AttackButton.NONE;

                            ActivateShield(false);

                            shieldActivated = false;
                            playerData.defense /= 5.0f;
                            playerData.specialDefense /= 5.0f;
                        }
                    }
                    #endregion
                    break;

                case PlayerData.CharacterType.ARCHER:
                    break;
            }
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

            //Stats restore
            playerData.RestoreStats();
        }
        else if (!arleadyDead)
        {
            Die();

            capsuleCollider.enabled = false;
            body.velocity = Vector3.zero;

            ActivateShield(false);

            arleadyDead = true;
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
                else if (Vector2.Angle(attackDirection, targetDirection) <= 45)
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

    IEnumerator Cooldown(float time)
    {
        isReadyToAttack = false;
        yield return new WaitForSeconds(time);
        isReadyToAttack = true;
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    //Animation
    #region
    //Activate/deactivate shield
    private void ActivateShield(bool isActive)
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
