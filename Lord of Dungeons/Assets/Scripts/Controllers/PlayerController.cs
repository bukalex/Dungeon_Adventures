using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class PlayerController : MonoBehaviour, IDefensiveMonoBehaviour, ITrainable
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
    private bool isTraining = true;

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
            UIManager.Instance.bossCounter.text = "0";
            UIManager.Instance.enemyCounter.text = "0";
            UIManager.Instance.levelCounter.text = "HUB";
            CheckpointManager.Instance.levelsPassed = 0;
            transform.position += Vector3.down * 15;
            playerData.isDead = false;
        }
    }
    void LateUpdate()
    {
        playerData.position = transform.position;
        playerData.isUsingMana = false;
        playerData.isUsingStamina = false;

        if (playerData.IsAlive() && !playerData.isStunned)
        {
            //Movement
            #region
            if (!CheatManager.Instance.ChatIsActive() && (TrainingManager.Instance == null || TrainingManager.Instance != null && !TrainingManager.Instance.movementBlocked))
            {
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
            }
            #endregion

            //Attacks
            #region
            if (!BattleManager.Instance.isUsingUI && !EventSystem.current.IsPointerOverGameObject() && (TrainingManager.Instance == null || TrainingManager.Instance != null && !TrainingManager.Instance.attacksBlocked))
            {
                //Left Mouse Button
                if (Input.GetKey(UIManager.Instance.keyCodes[6]) && !Input.GetKey(UIManager.Instance.keyCodes[7]))
                {
                    if (BattleManager.Instance.PlayerPerformAction(playerData, BattleManager.AttackButton.LMB))
                    {
                        body.velocity = movementDirection * playerData.speed * 0.5f;
                        AttackWithLMB();
                    }
                }

                //Right Mouse Button
                if (Input.GetKey(UIManager.Instance.keyCodes[7]))
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

            if (Input.GetKeyDown(UIManager.Instance.keyCodes[15]) || (isTalkingToNPC && Input.GetKeyDown(UIManager.Instance.keyCodes[12])))
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
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[15]) || (isLooting && Input.GetKeyDown(UIManager.Instance.keyCodes[12])))
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

            if (activeLootable != null)
            {
                if ((activeLootable.transform.position - transform.position).magnitude - playerData.colliderRadius - activeLootable.GetColliderRadius() > playerData.lootableDetectionRadius)
                {
                    activeLootable.BeingLooted(false);
                    activeLootable = null;
                }
            }
            #endregion

            //Stats restore
            playerData.RestoreStats();
        }
        else if (!alreadyDead && !playerData.isStunned)
        {
            Die();
            SoundManager.Instance.PlaySE(SESoundData.SE.PlayerDeath);

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

    public IEnumerator StartTraining()
    {
        //WSAD LShift
        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    TrainingManager.Instance.AddTask("Use " + UIManager.Instance.keyCodes[i].ToString() + " to move forward");
                    break;
                case 1:
                    TrainingManager.Instance.AddTask("Use " + UIManager.Instance.keyCodes[i].ToString() + " to move backward");
                    break;
                case 2:
                    TrainingManager.Instance.AddTask("Use " + UIManager.Instance.keyCodes[i].ToString() + " to move left");
                    break;
                case 3:
                    TrainingManager.Instance.AddTask("Use " + UIManager.Instance.keyCodes[i].ToString() + " to move right");
                    break;
                case 4:
                    TrainingManager.Instance.AddTask("Move and hold " + UIManager.Instance.keyCodes[i].ToString() + " to run");
                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        TrainingManager.Instance.movementBlocked = false;
        while (TrainingManager.Instance.HasUndoneTasks())
        {
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[0])) TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = true;
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[1])) TrainingManager.Instance.taskList.GetChild(1).GetComponent<Toggle>().isOn = true;
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[2])) TrainingManager.Instance.taskList.GetChild(2).GetComponent<Toggle>().isOn = true;
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[3])) TrainingManager.Instance.taskList.GetChild(3).GetComponent<Toggle>().isOn = true;
            if (Input.GetKey(UIManager.Instance.keyCodes[4]) && body.velocity.magnitude != 0) TrainingManager.Instance.taskList.GetChild(4).GetComponent<Toggle>().isOn = true;

            yield return null;
        }

        StartCoroutine(TrainingManager.Instance.RemoveTasks());
        yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);
        yield return new WaitForSeconds(0.5f);

        //Dialog
        TrainingManager.Instance.dialogPanel.SetActive(true);
        StartCoroutine(TrainingManager.Instance.StartTyping("Player:", TrainingManager.Instance.nameText));
        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
        StartCoroutine(TrainingManager.Instance.StartTyping("Hmm... I've got some coins in my pocket.", TrainingManager.Instance.textFieldObject));
        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
        yield return new WaitForSeconds(2.0f);
        TrainingManager.Instance.uiBlocked = false;
        TrainingManager.Instance.dialogPanel.SetActive(false);

        //TAB
        TrainingManager.Instance.AddTask("Open inventory (" + UIManager.Instance.keyCodes[13].ToString() + ")");
        while (TrainingManager.Instance.HasUndoneTasks())
        {
            if (Input.GetKeyDown(UIManager.Instance.keyCodes[13]))
            {
                TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = true;
                TrainingManager.Instance.uiBlocked = true;
            }
            yield return null;
        }

        StartCoroutine(TrainingManager.Instance.RemoveTasks());
        yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

        //Replace
        TrainingManager.Instance.AddTask("Drag and drop an item to replace it");
        TrainingManager.Instance.AddTask("Hold Left Shift click an item to replace it");
        while (TrainingManager.Instance.HasUndoneTasks())
        {
            TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = TrainingManager.Instance.itemWasDraggedAndMoved;
            TrainingManager.Instance.taskList.GetChild(1).GetComponent<Toggle>().isOn = TrainingManager.Instance.itemWasClickedAndMoved;
            yield return null;
        }

        StartCoroutine(TrainingManager.Instance.RemoveTasks(true));
        yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

        isTraining = false;
    }

    public bool IsTraining()
    {
        return isTraining;
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
