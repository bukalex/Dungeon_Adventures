using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
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

    private DirectionName directionName = DirectionName.FRONT;
    private Vector3 movementDirection = Vector3.zero;
    private float targetDistance;
    private bool alreadyDead = false;

    private int spriteOrder;
    private int enemyOrder;
    private float angle;

    public enum DirectionName { FRONT, BACK, LEFT, RIGHT }

    void Awake()
    {
        enemyParameters = Instantiate(enemyParametersOriginal);

        animator.runtimeAnimatorController = enemyParameters.animController;
        enemyParameters.transform = transform;
    }

    void Update()
    {
        enemyParameters.position = transform.position;
        enemyParameters.attackDirection = (enemyParameters.playerData.position - transform.position).normalized;
        enemyParameters.isUsingMana = false;
        enemyParameters.isUsingStamina = false;

        List<SpriteRenderer> renderers = DetectSprites<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.sortingLayerName.Equals(spriteRenderer.sortingLayerName))
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

        if (IsAlive() && !enemyParameters.isStunned)
        {
            //Movement and attack
            if (PlayerDetected())
            {
                ChangeDirection();

                if (enemyParameters.isBoss && 
                    enemyParameters.health <= enemyParameters.maxHealth * 0.5f && 
                    targetDistance <= BattleManager.Instance.GetAttackRange(enemyParameters.type, BattleManager.AttackButton.RMB) &&
                    BattleManager.Instance.EnemyPerformAction(enemyParameters, BattleManager.AttackButton.RMB))
                {
                    SuperAttack();
                }
                else
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
            else
            {
                Stop();
                body.velocity = Vector3.zero;
            }

            //Stats restore
            RestoreStats();
        }
        else if (!alreadyDead && !enemyParameters.isStunned)
        {
            capsuleCollider.enabled = false;
            body.velocity = Vector3.zero;
            Die();

            if (enemyParameters.type == EnemyParameters.EnemyType.GHOST)
            {
                StartCoroutine(DelayedDie());
            }

            alreadyDead = true;
        }
    }

    private void Seek()
    {
        movementDirection = (enemyParameters.playerData.position - transform.position).normalized;
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
        targetDistance = (enemyParameters.playerData.position - transform.position).magnitude - enemyParameters.playerData.colliderRadius - enemyParameters.colliderRadius;
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

    private IEnumerator DelayedDie()
    {
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
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
    }

    private void SuperAttack()
    {
        Stop();
        animator.SetTrigger("superAttack");
    }

    private void Die()
    {
        Stop();
        animator.SetTrigger("isDead");
    }
    #endregion
}
