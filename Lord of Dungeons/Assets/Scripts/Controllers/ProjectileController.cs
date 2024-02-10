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
    private IAttackObject attackObject;
    private IDefenseObject defenseObject;
    private AttackParameters attack;

    public void Launch(string parentTag, IAttackObject attackObject, IDefenseObject defenseObject, AttackParameters attack, Vector3 direction)
    {
        this.parentTag = parentTag;
        this.attackObject = attackObject;
        this.defenseObject = defenseObject;
        this.attack = attack;

        body.velocity = direction.normalized * speed;
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
                        BattleManager.Instance.ProjectileHit(attackObject, defenseObject, attack);
                    }
                    break;

                case "Player":
                    if (targetTag.Equals("Enemy") || targetTag.Equals("Object"))
                    {
                        BattleManager.Instance.ProjectileHit(attackObject, defenseObject, attack);
                    }
                    break;
            }

            if (!targetTag.Equals(parentTag))
            {
                Destroy(gameObject);
            }
        }
    }
}
