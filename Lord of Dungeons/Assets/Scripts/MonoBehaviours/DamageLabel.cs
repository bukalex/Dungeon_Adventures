using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLabel : MonoBehaviour
{
    private Vector3 positionOffset;
    private IDefenseObject defenseObject;

    void Start()
    {
        Destroy(gameObject, 0.8f);
    }

    void Update()
    {
        positionOffset += Vector3.up * Time.deltaTime;
        transform.position = Camera.main.WorldToScreenPoint(defenseObject.GetPosition() + Vector3.up * 1.6f + positionOffset);
    }

    public void SetDefenseObject(IDefenseObject defenseObject)
    {
        this.defenseObject = defenseObject;
    }
}
