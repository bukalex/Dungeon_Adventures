using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Scene")]
public class SceneData : ScriptableObject
{
    public int sceneNextNumber = 1;
    public Vector3 playerPosition;
}
