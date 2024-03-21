using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlacksmithItemHolder : MonoBehaviour, IPointerClickHandler
{
    public BlacksmithItemHolder thisItemHolder;
    public int ItemID;
    public int ChildPos;
    public RecipeCollection recipe;

    public GameObject[] materialDisplays;
    public GameObject itemIcon;


    private void Update()
    {
        transform.SetSiblingIndex(ChildPos);
    }

    public IEnumerator offButton(int interval)
    {
        yield return new WaitForSeconds(interval);
        BlacksmithUI.Instance.craftItemButton.interactable = false;
        BlacksmithUI.Instance.buttonText.text = "Press on any item to craft";
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        BlacksmithUI.Instance.craftItemButton.interactable = true;
        BlacksmithUI.Instance.buttonText.text = "Craft" + recipe.GetItemName(ItemID);
        BlacksmithUI.Instance.currentItemID = ItemID;
        BlacksmithUI.Instance.currentItemHodler = thisItemHolder;
        offButton(15);
    }
}
