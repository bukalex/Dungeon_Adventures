using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithItemHolder : MonoBehaviour
{
    public int ItemID;
    public int ChildPos;

    public GameObject[] materialDisplays;
    public GameObject itemIcon;

    private void Update()
    {
        transform.SetSiblingIndex(ChildPos);
    }
}
