using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HouseDoor : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Transform oppositeSide;
    [SerializeField]
    private bool isInside;

    [SerializeField]
    private GameObject interactIconPrefab;
    private GameObject interactIcon;

    void Start()
    {
        interactIcon = Instantiate(interactIconPrefab, Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2), Quaternion.identity, GameObject.FindGameObjectWithTag("MainCanvas").transform);
        interactIcon.transform.SetSiblingIndex(0);
        interactIcon.SetActive(false);
    }

    void Update()
    {
        if (interactIcon.activeSelf && Input.GetKeyDown(UIManager.Instance.keyCodes[15]) && (!isInside || isInside && TrainingManager.Instance.canGoOut))
        {
            DataManager.Instance.playerData.transform.position = oppositeSide.position;
            TrainingManager.Instance.isInside = !isInside;
        }
    }

    public void ShowButton()
    {
        if (interactIcon != null && (!isInside || isInside && TrainingManager.Instance.canGoOut))
        {
            interactIcon.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
            interactIcon.GetComponentInChildren<TMP_Text>().text = UIManager.Instance.textKeys[15].text;
            interactIcon.SetActive(true);
        }
    }

    public void HideButton()
    {
        if (interactIcon != null) interactIcon.SetActive(false);
    }
}
