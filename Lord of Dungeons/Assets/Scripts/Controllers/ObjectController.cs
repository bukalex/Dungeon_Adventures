using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour, IDefensiveMonoBehaviour
{
    [SerializeField]
    private ObjectParameters objectParametersOriginal;
    public ObjectParameters objectParameters { get; private set; }

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private AttackParameters attack;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private CapsuleCollider2D capsuleCollider;

    [SerializeField]
    private LootRandomizer randomizer;

    private bool alreadyBusted = false;

    private int spriteOrder;
    private int enemyOrder;
    private float angle;

    void Awake()
    {
        objectParameters = Instantiate(objectParametersOriginal);
        animator.runtimeAnimatorController = objectParameters.animController;
        objectParameters.position = transform.position;
    }

    void Update()
    {
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

        if (!IsIntact() && !alreadyBusted)
        {
            Bust();
            alreadyBusted = true;
            randomizer.DropItems();
        }
    }

    public bool IsIntact()
    {
        return objectParameters.health > 0;
    }

    private void Bust()
    {

        animator.SetTrigger("isBroken");
        capsuleCollider.enabled = false;
        GetComponentInChildren<PolygonCollider2D>().enabled = false;
    }

    private void Attack()
    {
        animator.SetTrigger("isAttacking");
        //However you deal damage with battle manager here
        Debug.Log("Hit");
    }

    private List<T> DetectSprites<T>()
    {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, objectParameters.colliderRadius * 2);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (objectParameters.isTrap == true)
        {
            Attack();
        }
    }

    public IDefenseObject GetDefenseObject()
    {
        return objectParameters;
    }
}
