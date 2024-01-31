using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpPoison : MonoBehaviour
{
    private Item item;

    [SerializeField] private SpriteRenderer sr;

    public void Initialize(Item item)
    {
        this.item = item;
        sr.sprite = item.image;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InventoryManager.instance.AddItem(item);
            Destroy(gameObject);
        }
    }
}
