using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDefensiveMonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private CapsuleCollider2D capsuleCollider;

    private Vector3 movementDirection = Vector3.zero;

    private bool alreadyDead = false;
    private bool isTalkingToNPC = false;
    private bool isLooting = false;
    private bool isGoingBackward = false;

    private NPCController activeNPC = null;
    private LootableController activeLootable = null;
    private List<IInteractable> interactables = new List<IInteractable>();

    private void Start()
    {
        playerData.position = transform.position;
        playerData.transform = transform;
        animator.runtimeAnimatorController = playerData.animController;
        if (playerData.isDead)
        {
            transform.position += Vector3.down * 15;
            playerData.isDead = false;
        }
    }
    void Update()
    {
        playerData.position = transform.position;
        playerData.isUsingMana = false;
        playerData.isUsingStamina = false;

        if (playerData.IsAlive() && !playerData.isStunned)
        {
            //Movement
            #region
            movementDirection = new Vector3(-System.Convert.ToInt32(Input.GetKey(UIManager.Instance.keyCodes[2])) + System.Convert.ToInt32(Input.GetKey(UIManager.Instance.keyCodes[3])),
                -System.Convert.ToInt32(Input.GetKey(UIManager.Instance.keyCodes[1])) + System.Convert.ToInt32(Input.GetKey(UIManager.Instance.keyCodes[0])), 0).normalized;

            if (!isGoingBackward)
            {
                ChangeDirection();
            }

            if (movementDirection.magnitude != 0)
            {
                playerData.attackDirection = movementDirection;
                isGoingBackward = false;

                if (Input.GetKey(UIManager.Instance.keyCodes[4]) && BattleManager.Instance.PlayerPerformShift(playerData))
                {
                    Run();
                    body.velocity = movementDirection * playerData.speed * playerData.sprintFactor;
                }
                else if (Input.GetKey(UIManager.Instance.keyCodes[5]))
                {
                    Walk();
                    body.velocity = movementDirection * playerData.speed * playerData.slowWalkFactor;

                    playerData.attackDirection = -movementDirection;
                    isGoingBackward = true;
                }
                else
                {
                    Walk();
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
            if (Input.GetKey(UIManager.Instance.keyCodes[6]) && !Input.GetKey(UIManager.Instance.keyCodes[7]) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (BattleManager.Instance.PlayerPerformAction(playerData, BattleManager.AttackButton.LMB))
                {
                    body.velocity = movementDirection * playerData.speed * 0.5f;
                    AttackWithLMB();
                }
            }

            //Right Mouse Button
            if (Input.GetKey(UIManager.Instance.keyCodes[7]) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (BattleManager.Instance.PlayerPerformAction(playerData, BattleManager.AttackButton.RMB))
                {
                    
                }
            }

            //R Button
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[8]))
            {
                if (BattleManager.Instance.PlayerPerformAction(playerData, BattleManager.AttackButton.R))
                {

                }
            }

            //F Button
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[9]))
            {
                if (BattleManager.Instance.PlayerPerformAction(playerData, BattleManager.AttackButton.F))
                {

                }
            }

            //C Button
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[10]))
            {
                if (BattleManager.Instance.PlayerPerformAction(playerData, BattleManager.AttackButton.C))
                {

                }
            }

            //V Button
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[11]))
            {
                if (BattleManager.Instance.PlayerPerformAction(playerData, BattleManager.AttackButton.V))
                {

                }
            }
            #endregion

            //NPC interaction
            #region
            foreach (IInteractable interactable in interactables)
            {
                interactable.HideButton();
            }
            interactables = DetectTargets<IInteractable>(playerData.npcDetectionRadius + playerData.colliderRadius, false);
            foreach (IInteractable interactable in interactables)
            {
                interactable.ShowButton();
            }

            if (Input.GetKeyDown(UIManager.Instance.keyCodes[15]))
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
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[15]))
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
                    activeLootable.BeingLooted(isLooting);
                }
            }
            #endregion

            //Stats restore
            playerData.RestoreStats();
        }
        else if (!alreadyDead && !playerData.isStunned)
        {
            Die();

            capsuleCollider.enabled = false;
            body.velocity = Vector3.zero;

            alreadyDead = true;
            playerData.isDead = true;
            StartCoroutine(Respawn());
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

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2);
        InventoryManager.Instance.LoseInventory();
        SceneManager.LoadScene("HUB");
    }

    public IDefenseObject GetDefenseObject()
    {
        return playerData;
    }

    //Animation
    #region
    //Change movement direction
    private void ChangeDirection()
    {
        animator.SetFloat("Horizontal", playerData.attackDirection.x);
        animator.SetFloat("Vertical", playerData.attackDirection.y);
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
