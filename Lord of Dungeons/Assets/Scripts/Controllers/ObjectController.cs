using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    private ObjectParameters objectParametersOriginal;
    public ObjectParameters objectParameters { get; private set; }

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private CapsuleCollider2D capsuleCollider;

    private bool alreadyBusted = false;
  
    void Awake()
    {
        objectParameters = Instantiate(objectParametersOriginal);
        animator.runtimeAnimatorController = objectParameters.animController;
    }

    void Update()
    {
        if (!IsIntact() && !alreadyBusted)
        {
            Bust();
            alreadyBusted = true;
        }
    }

    public bool IsIntact()
    {
        return objectParameters.health > 0;
    }

    private void Bust()
    {
        Instantiate(objectParametersOriginal.dropPrefab, transform.position, new Quaternion());
        animator.SetTrigger("isBroken");
        capsuleCollider.enabled = false;
    }
}
