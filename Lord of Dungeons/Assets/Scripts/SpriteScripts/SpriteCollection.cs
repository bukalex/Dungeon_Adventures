using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/SpriteCollection")]
public class SpriteCollection : ScriptableObject
{
    public List<ItemSprite> Armors = new List<ItemSprite>();
    public List<ItemSprite> Bows = new List<ItemSprite>();
    public List<ItemSprite> Swords = new List<ItemSprite>();
}
