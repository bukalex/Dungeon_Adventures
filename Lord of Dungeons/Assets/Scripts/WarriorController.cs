using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{
    [SerializeField] float meleeRange = 0.8f;
    [SerializeField] float meleeCooldown = 0.625f;

    private bool isReadyToAttack = true;
    private Rigidbody2D body;
    private GeneralCharacterController characterController;
    private WarriorAnimationController animationController;

    private enum AttackType { BASIC, SPECIAL }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        characterController = GetComponent<GeneralCharacterController>();

        animationController = GetComponentInChildren<WarriorAnimationController>();
    }

    void Update()
    {
        //Attack
        if (isReadyToAttack)
        {
            if (Input.GetMouseButton(0))//Basic attack
            {
                PerformAttack(AttackType.BASIC);
                StartCoroutine(Cooldown(meleeCooldown));
            }
            else if (Input.GetMouseButton(1))//Special attack
            {
                PerformAttack(AttackType.SPECIAL);
                StartCoroutine(Cooldown(meleeCooldown));
            }
        }
    }

    private void PerformAttack(AttackType attackType)
    {
        //Find enemies within attack range
        List<GeneralEnemyController> enemies = DetectEnemies();

        //Fight enemies
        foreach (GeneralEnemyController enemy in enemies)
        {
            switch (attackType)
            {
                case AttackType.BASIC:
                    enemy.OnHit();
                    break;

                case AttackType.SPECIAL:
                    enemy.OnHit();
                    break;
            }
        }
    }

    //Coroutine will allow us to perform attacks with certain rate
    IEnumerator Cooldown(float time)
    {
        isReadyToAttack = false;
        yield return new WaitForSeconds(time);
        isReadyToAttack = true;
    }

    //Detect enemies in the 90 degree sector in front of the player
    private List<GeneralEnemyController> DetectEnemies()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, meleeRange);
        List<GeneralEnemyController> enemies = new List<GeneralEnemyController>();

        foreach (Collider2D target in targets)
        {
            if (target.tag.Equals("Enemy"))
            {
                Vector2 attackDirection = characterController.GetAttackDirection();
                Vector2 targetDirection = target.transform.position - transform.position;

                if (Vector2.Angle(attackDirection, targetDirection) <= 45)
                {
                    enemies.Add(target.GetComponent<GeneralEnemyController>());
                }
            }
        }

        return enemies;
    }

    //Call this method when enemy hits player
    public void OnHit()
    {
        animationController.Hurt();
    }
}
