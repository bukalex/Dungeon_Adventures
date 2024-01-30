using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField]
    private ItemParameters itemParameters;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Animator animator;

    void Awake()
    {
        spriteRenderer.sprite = itemParameters.sprite;

        if (animator != null)
        {
            animator.runtimeAnimatorController = itemParameters.animController;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && itemParameters.category == ItemParameters.ItemCategory.RESOURCES)
        {
            itemParameters.playerData.resources[itemParameters.resourceType]++;

            Destroy(gameObject);
        }
    }
}
