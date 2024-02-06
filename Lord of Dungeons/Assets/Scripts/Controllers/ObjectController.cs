using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    private ObjectParameters objectParameters;

    [SerializeField] 
    private GameObject setObject;

    [SerializeField]
    private GameObject dialogueWindow;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private CapsuleCollider2D capsuleCollider;
  
    private float health;

    void Awake()
    {
        animator.runtimeAnimatorController = objectParameters.animController;
        dialogueWindow.SetActive(false);
        health = objectParameters.health;
    }

    private void Update()
    {
        if (GetObjectType(setObject) == ObjectParameters.ObjectType.Interactable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                dialogueWindow.SetActive(true);
            }
            else
            {
                dialogueWindow.SetActive(false);
            }
        }
    }

    public bool isIntact()
    {
        return health > 0;
    }

    public void DealDamage(PlayerData.AttackType attackType, float damage, float attackWeight)
    {
        switch (attackType)
        {
            case PlayerData.AttackType.BASIC:
                damage *= 1.0f + (attackWeight - objectParameters.defense) * 0.05f;
                break;

            case PlayerData.AttackType.SPECIAL:
                damage *= 1.0f + (attackWeight - objectParameters.specialDefense) * 0.05f;
                break;
        }

        if (damage < 1.0f)
        {
            damage = 1.0f;
        }

        Debug.Log("Pot was hit. Damage: " + damage);
        health -= damage;

        if (!isIntact())
        {
            Bust();
        }
    }

    private void Bust()
    {
        Instantiate(objectParameters.dropPrefab, transform.position, new Quaternion());
        animator.SetTrigger("isBroken");
    }

    private ObjectParameters.ObjectType GetObjectType(GameObject newGameObj)
    {
        ObjectParameters objectData = newGameObj.GetComponent<ObjectParameters>();
        if (objectData.isInteractable == true)
        {
            objectData.objectType = ObjectParameters.ObjectType.Interactable;
            return objectData.objectType;
        }

        return ObjectParameters.ObjectType.NONE;
    }
}
