using UnityEngine;

[CreateAssetMenu(fileName = "New Trap Parameters", menuName = "ScriptableObjects/Trap parameters")]
public class TrapParameters : ScriptableObject
{
    public float timeOffset;
    public float duration;
    public float cooldown;
    public float damage;
}
