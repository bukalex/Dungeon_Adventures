using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectController : MonoBehaviour, IDefensiveMonoBehaviour, ITrainable
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

    [SerializeField]
    private List<ObjectController> barricade;

    private bool alreadyBusted = false;
    private bool isTraining = true;

    private int spriteOrder;
    private int enemyOrder;
    private float angle;

    void Awake()
    {
        objectParameters = Instantiate(objectParametersOriginal);
        if (animator != null) animator.runtimeAnimatorController = objectParameters.animController;
        objectParameters.position = transform.position;

        if (objectParameters.isBarricade)
        {
            foreach (ObjectController objectController in barricade)
            {
                objectController.GetComponentInChildren<PolygonCollider2D>().enabled = false;
            }
        }
    }

    void Update()
    {
        if (spriteRenderer != null)
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
        }

        if (!IsIntact() && !alreadyBusted)
        {
            Bust();
            alreadyBusted = true;
            if (randomizer != null) randomizer.DropItems();
        }
    }

    public bool IsIntact()
    {
        return objectParameters.health > 0;
    }

    private void Bust()
    {
        if (objectParameters.isBarricade)
        {
            foreach (ObjectController objectController in barricade)
            {
                objectController.objectParameters.health = 0;
            }
        }
        else
        {
            animator.SetTrigger("isBroken");
            capsuleCollider.enabled = false;
            GetComponentInChildren<PolygonCollider2D>().enabled = false;
        }
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

    public IEnumerator StartTraining()
    {
        TrainingManager.Instance.uiBlocked = false;
        TrainingManager.Instance.movementBlocked = false;
        TrainingManager.Instance.attacksBlocked = false;

        //Dialog
        TrainingManager.Instance.dialogPanel.SetActive(true);
        TrainingManager.Instance.nameText.text = "";
        TrainingManager.Instance.textFieldObject.text = "";
        StartCoroutine(TrainingManager.Instance.StartTyping("Player:", TrainingManager.Instance.nameText));
        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
        StartCoroutine(TrainingManager.Instance.StartTyping("Time to try the new sword.", TrainingManager.Instance.textFieldObject));
        yield return new WaitWhile(() => TrainingManager.Instance.isTyping);
        yield return new WaitForSeconds(2.0f);
        TrainingManager.Instance.dialogPanel.SetActive(false);

        TrainingManager.Instance.AddTask("Destroy the barricade");
        while (TrainingManager.Instance.HasUndoneTasks())
        {
            TrainingManager.Instance.taskList.GetChild(0).GetComponent<Toggle>().isOn = alreadyBusted;
            yield return null;
        }

        StartCoroutine(TrainingManager.Instance.RemoveTasks());
        yield return new WaitWhile(() => TrainingManager.Instance.isRemovingTasks);

        isTraining = false;
    }

    public bool IsTraining()
    {
        return isTraining;
    }
}
