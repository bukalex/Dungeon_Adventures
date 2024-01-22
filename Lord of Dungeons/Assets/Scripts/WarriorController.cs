using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    [SerializeField] float meleeRange = 0.8f;
    [SerializeField] float meleeCooldown = 0.625f;

    private bool isReadyToAttack = true;

    private enum AttackType { BASIC, SPECIAL }

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

    private List<GeneralEnemyController> DetectEnemies()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, meleeRange);
        List<GeneralEnemyController> enemies = new List<GeneralEnemyController>();

        foreach (Collider2D target in targets)
        {
            if (target.tag.Equals("Enemy"))
            {
                enemies.Add(target.GetComponent<GeneralEnemyController>());
            }
        }

        return enemies;
    }

    //Call this method when enemy hits player
    public void OnHit()
    {
        Debug.Log("Player was hit");
    }
}
