using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Scene")]
public class Scene : ScriptableObject
{
    public int sceneNextNumber = 1;
    public Vector3 playerPosition;
}
