using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] float meleeRange = 1.0f;
    [SerializeField] float meleeCooldown = 1.0f;
    [SerializeField] float damage = 10.0f;

    private bool isReadyToAttack = true;
    private Rigidbody2D body;
    private GeneralEnemyController enemyController;
    private EnemyAnimationController animationController;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        enemyController = GetComponent<GeneralEnemyController>();
        animationController = GetComponentInChildren<EnemyAnimationController>();
    }

    void Update()
    {
        if (enemyController.isAlive())
        {
            if (enemyController.player != null)
            {
                if (enemyController.inAttackRange(meleeRange))
                {
                    enemyController.Stop();

                    if (isReadyToAttack)
                    {
                        PerformAttack();
                        StartCoroutine(Cooldown(meleeCooldown));
                    }
                }
                else
                {
                    enemyController.RunToPlayer();
                }
            }
        }

        //check if a guard is dead
        if (enemyController.health <= 0)
        {
            //spawn a coin after his death
            Instantiate(coinPrefab);
        }
    }

    private void PerformAttack()
    {
        //Fight player
        animationController.Attack();
        enemyController.player.DealDamage(damage);
    }

    //Coroutine will allow us to perform attacks with certain rate
    IEnumerator Cooldown(float time)
    {
        isReadyToAttack = false;
        yield return new WaitForSeconds(time);
        isReadyToAttack = true;
    }

}
