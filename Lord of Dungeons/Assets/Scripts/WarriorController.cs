using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarriorController : MonoBehaviour
{
    LongRangeAttack lrg;

    [SerializeField] float meleeRange = 1.0f;
    [SerializeField] float meleeCooldown = 0.625f;
    [SerializeField] float damage = 15.0f;

    public int score = 0;  // Score number of coin
    public Text scoreText;
    private bool isReadyToAttack = true;
    private Rigidbody2D body;
    private GeneralCharacterController characterController;

    private enum AttackType { BASIC, SPECIAL }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        characterController = GetComponent<GeneralCharacterController>();
    }

    void Update()
    {
        if (characterController.isAlive())
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
    }

    private void PerformAttack(AttackType attackType)
    {
        //Find enemies within attack range
        List<GeneralEnemyController> enemies = characterController.DetectEnemies(meleeRange);

        //Fight enemies
        foreach (GeneralEnemyController enemy in enemies)
        {
            switch (attackType)
            {
                case AttackType.BASIC:
                    enemy.DealDamage(damage);
                    break;

                case AttackType.SPECIAL:
                    enemy.DealDamage(damage * 2);
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
   

    public void UpdateScore(int coinValue)
    {
        score += coinValue;
        scoreText.text = score.ToString(); 
    }
}
