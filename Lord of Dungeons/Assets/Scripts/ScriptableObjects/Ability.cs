using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
    [Header("Visual properties")]
    public Sprite maskSprite;
    public Sprite backgroundSprite;
    public Mask mask;
}
