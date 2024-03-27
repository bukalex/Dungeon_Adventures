using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDefensiveMonoBehaviour
{
    [SerializeField]
    private EnemyParameters enemyParametersOriginal;
    public EnemyParameters enemyParameters { get; private set; }

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private CapsuleCollider2D capsuleCollider;

    [SerializeField]
    private LootRandomizer randomizer;

    private Vector3 lastPlayerPosition;
    private Vector3 movementDirection = Vector3.zero;
    private float targetDistance;
    private bool alreadyDead = false;

    private int spriteOrder;
    private int enemyOrder;
    private float angle;

    public enum DirectionName { FRONT, BACK, LEFT, RIGHT }

    // Patrol behavior
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;

    private int currentPatrolIndex = 0;
    private Vector3 currentTarget;
    private bool isPatrolling = true;
    private Transform player;

    void Awake()
    {
        // Patrol behavior
        currentTarget = patrolPoints[currentPatrolIndex].position;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        

        enemyParameters = Instantiate(enemyParametersOriginal);

        lastPlayerPosition = transform.position;
        animator.runtimeAnimatorController = enemyParameters.animController;
        enemyParameters.transform = transform;

        if (enemyParameters.isBoss) UIManager.Instance.bossCounter.text = (int.Parse(UIManager.Instance.bossCounter.text) + 1).ToString();
        else UIManager.Instance.enemyCounter.text = (int.Parse(UIManager.Instance.enemyCounter.text) + 1).ToString();
    }

    void Update()
    {

        // Patrol behavior
        if (isPatrolling)
        {
            Patrol();
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (PlayerDetected())
        {
            isPatrolling = false;
            if (targetDistance <= enemyParameters.attack)
            {
                Attack();
            }
        }
        else
        {
            Patrol();
            animator.SetBool("isWalking", true);
        }


        enemyParameters.position = transform.position;
        enemyParameters.attackDirection = (enemyParameters.playerData.position - transform.position).normalized;
        enemyParameters.isUsingMana = false;
        enemyParameters.isUsingStamina = false;

        List<SpriteRenderer> renderers = DetectSprites<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer != null && renderer.sortingLayerName.Equals(spriteRenderer.sortingLayerName))
            {
                angle = Vector2.SignedAngle(Vector3.right, renderer.transform.parent.position - transform.position);
                if (135 >= angle && angle > 45)
                {
                    spriteOrder = renderer.sortingOrder;
                    enemyOrder = spriteRenderer.sortingOrder;
                    if (spriteOrder > enemyOrder)
                    {
                        spriteRenderer.sortingOrder = spriteOrder;
                        renderer.sortingOrder = enemyOrder;
                    }
                    else if (spriteOrder == enemyOrder)
                    {
                        spriteRenderer.sortingOrder += 1;
                    }
                }
                else if (-135 <= angle && angle < -45)
                {
                    spriteOrder = renderer.sortingOrder;
                    enemyOrder = spriteRenderer.sortingOrder;
                    if (spriteOrder < enemyOrder)
                    {
                        spriteRenderer.sortingOrder = spriteOrder;
                        renderer.sortingOrder = enemyOrder;
                    }
                    else if (spriteOrder == enemyOrder)
                    {
                        renderer.sortingOrder += 1;
                    }
                }
            }
        }

        if (IsAlive())
        {
            //Movement and attack
            if (PlayerDetected())
            {
                ChangeDirection();
                lastPlayerPosition = enemyParameters.playerData.position;

                if (enemyParameters.isBoss && 
                    enemyParameters.health <= enemyParameters.maxHealth * 0.5f && 
                    targetDistance <= BattleManager.Instance.GetAttackRange(enemyParameters.type, BattleManager.AttackButton.RMB) &&
                    BattleManager.Instance.EnemyPerformAction(enemyParameters, BattleManager.AttackButton.RMB))
                {
                    SuperAttack();
                }
                else if (!enemyParameters.isAttacking)
                {
                    if (targetDistance > BattleManager.Instance.GetAttackRange(enemyParameters.type, BattleManager.AttackButton.LMB))
                    {
                        Run();
                        Seek();
                        AvoidObstacles();
                    }
                    else
                    {
                        if (enemyParameters.type == EnemyParameters.EnemyType.GHOST && targetDistance < BattleManager.Instance.GetAttackRange(enemyParameters.type, BattleManager.AttackButton.LMB) * 0.75f)
                        {
                            Run();
                            Flee();
                            AvoidObstacles();
                        }
                        else
                        {
                            Stop();
                            body.velocity = Vector3.zero;
                        }

                        if (BattleManager.Instance.EnemyPerformAction(enemyParameters, BattleManager.AttackButton.LMB))
                        {
                            Attack();
                        }
                    }
                }
            }
            else if ((lastPlayerPosition - transform.position).magnitude <= 0.5)
            {
                Stop();
                body.velocity = Vector3.zero;
            }
            else
            {
                Run();
                Seek();
                AvoidObstacles();
            }

            //Stats restore
            RestoreStats();
        }
        else if (!alreadyDead && !enemyParameters.isStunned)
        {
            capsuleCollider.enabled = false;
            GetComponentInChildren<PolygonCollider2D>().enabled = false;
            body.velocity = Vector3.zero;
            Die();

            if (enemyParameters.isBoss) UIManager.Instance.bossCounter.text = (int.Parse(UIManager.Instance.bossCounter.text) - 1).ToString();
            else UIManager.Instance.enemyCounter.text = (int.Parse(UIManager.Instance.enemyCounter.text) - 1).ToString();

            randomizer.DropItems();
            alreadyDead = true;
            Destroy(gameObject, 1);
        }
    }

    // Patrol behavior
    private void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            currentTarget = patrolPoints[currentPatrolIndex].position;
        }
    }

    private void Seek()
    {
        movementDirection = (lastPlayerPosition - transform.position).normalized;
    }

    private void Flee()
    {
        movementDirection = (transform.position - enemyParameters.playerData.position).normalized;
    }

    private void AvoidObstacles()
    {
        if (CastWhisker(enemyParameters.whiskerAngle) || CastWhisker(enemyParameters.whiskerAngle * 2))
        {
            movementDirection = Quaternion.Euler(0, 0, -enemyParameters.rotationSpeed) * movementDirection.normalized;
        }
        else if (CastWhisker(-enemyParameters.whiskerAngle) || CastWhisker(-enemyParameters.whiskerAngle * 2))
        {
            movementDirection = Quaternion.Euler(0, 0, enemyParameters.rotationSpeed) * movementDirection.normalized;
        }

        body.velocity = movementDirection * enemyParameters.speed;
    }

    private bool CastWhisker(float angle)
    {
        Vector2 whiskerDirection = Quaternion.Euler(0, 0, angle) * movementDirection;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, whiskerDirection, enemyParameters.whiskerLength);
        Debug.DrawRay(transform.position, whiskerDirection, Color.red);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && !hit.collider.isTrigger && hit.collider.gameObject != gameObject && hit.transform.tag != "Player")
            {
                return true;
            }
        }

        return false;
    }

    private bool PlayerDetected()
    {
        targetDistance = (enemyParameters.playerData.position - transform.position).magnitude - 
            enemyParameters.playerData.colliderRadius - 
            enemyParameters.colliderRadius;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, 
            (enemyParameters.playerData.position - transform.position).normalized, 
            (enemyParameters.playerData.position - transform.position).magnitude);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && !hit.collider.isTrigger && hit.collider.gameObject != gameObject && hit.transform.tag != "Player")
            {
                return false;
            }
        }

        return targetDistance <= enemyParameters.detectionRadius && enemyParameters.playerData.IsAlive();
    }

    public bool IsAlive()
    {
        return enemyParameters.health > 0;
    }

    private void RestoreStats()
    {
        enemyParameters.health = Mathf.Clamp(enemyParameters.health + enemyParameters.healthRestoreRate * Time.deltaTime, 0, enemyParameters.maxHealth);
        if (!enemyParameters.isUsingMana)
        {
            enemyParameters.mana = Mathf.Clamp(enemyParameters.mana + enemyParameters.manaRestoreRate * Time.deltaTime, 0, enemyParameters.maxMana);
        }
        if (!enemyParameters.isUsingStamina)
        {
            enemyParameters.stamina = Mathf.Clamp(enemyParameters.stamina + enemyParameters.staminaRestoreRate * Time.deltaTime, 0, enemyParameters.maxStamina);
        }
    }

    private List<T> DetectSprites<T>()
    {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, enemyParameters.colliderRadius * 2);
        List<T> targets = new List<T>();

        foreach (Collider2D target in possibleTargets)
        {
            if (target.GetComponentInChildren<T>() != null)
            {
                targets.Add(target.GetComponentInChildren<T>());
            }
        }

        return targets;
    }

    public IDefenseObject GetDefenseObject()
    {
        return enemyParameters;
    }

    //Animation
    #region
    //Change movement direction
    private void ChangeDirection()
    {
        animator.SetFloat("Horizontal", enemyParameters.attackDirection.x);
        animator.SetFloat("Vertical", enemyParameters.attackDirection.y);

        if (Mathf.Abs(Vector2.SignedAngle(Vector3.right, enemyParameters.playerData.position - transform.position)) <= 45)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    // Patrol behvaior
    public void StartPatrolling()
    {
        isPatrolling = true;
    }

    public void StopPatrolling()
    {
        isPatrolling = false;
    }

    private void Stop()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isConfused", false);
    }

    private void Walk()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
    }

    private void Run()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", true);
    }

    private void Attack()
    {
        Stop();
        animator.SetTrigger("attack");

        // Stop patrolling and attacks player
        isPatrolling = false;
        animator.SetTrigger("attack");
    }

    private void SuperAttack()
    {
        Stop();
        animator.SetTrigger("superAttack");

        // Stop patrolling and attacks player
        isPatrolling = false;
        animator.SetTrigger("attack");
    }

    private void Die()
    {
        Stop();
        animator.SetTrigger("isDead");
    }
    #endregion
}
