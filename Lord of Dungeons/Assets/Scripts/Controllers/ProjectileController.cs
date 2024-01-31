using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.5f;

    [SerializeField]
    private Rigidbody2D body;

    private string parentTag;
    private PlayerData.AttackType attackType;
    private float damage;
    private float attackWeight;

    public void Launch(Vector3 direction, string parentTag, PlayerData.AttackType attackType, float damage, float attackWeight)
    {
        body.velocity = direction.normalized * speed;
        this.parentTag = parentTag;
        this.attackType = attackType;
        this.damage = damage;
        this.attackWeight = attackWeight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string targetTag = collision.transform.tag;

        if (!targetTag.Equals(parentTag))
        {
            switch (parentTag)
            {
                case "Enemy":
                    if (targetTag.Equals("Player"))
                    {
                        collision.GetComponent<PlayerController>().GetPlayerData().DealDamage(attackType, damage, attackWeight);
                        Destroy(gameObject);
                    }
                    break;

                case "Player":
                    if (targetTag.Equals("Enemy"))
                    {
                        collision.GetComponent<EnemyController>().DealDamage(attackType, damage, attackWeight);
                        Destroy(gameObject);
                    }
                    break;
            }
        }
    }
}
