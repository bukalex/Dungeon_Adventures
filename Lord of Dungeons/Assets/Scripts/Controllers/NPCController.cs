using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField]
    private NPCParameters npcParameters;

    [SerializeField]
    private GameObject dialogWindow;

    void Start()
    {

    }

    void Update()
    {

    }

    public void InteractWithPlayer(bool isActive)
    {
        dialogWindow.SetActive(isActive);
    }

    public float GetColliderRadius()
    {
        return npcParameters.colliderRadius;
    }
}
