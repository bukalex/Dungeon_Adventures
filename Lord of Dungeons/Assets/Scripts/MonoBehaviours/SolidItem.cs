using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidItem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private Item item;

    public void Initialize(Item item)
    {
        spriteRenderer.sprite = item.image;
        this.item = item;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (item != null && collision.isTrigger && collision.transform.parent != null && collision.transform.parent.tag == "Player")
        {
            if (InventoryManager.Instance.AddItem(item, InventoryManager.Instance.toolBar, InventoryManager.Instance.internalInventorySlots)) Destroy(gameObject);
        }
    }
}
