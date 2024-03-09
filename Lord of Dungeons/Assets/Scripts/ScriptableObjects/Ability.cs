using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;

    [Header("Data")]
    public AttackParameters attackParameters;

    [Header("Visual properties")]
    public Sprite maskSprite;
    public Sprite backgroundSprite;
    public Mask mask;
}
