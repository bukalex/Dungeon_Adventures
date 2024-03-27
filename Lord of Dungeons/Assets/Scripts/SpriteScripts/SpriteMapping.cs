using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteMapping : MonoBehaviour
{
    public string SpriteName;
    public string BodyName;
    public Sprite FindSprite(SpriteAtlas sprites)
    {
        return sprites.GetSprite(SpriteName);
    }
}
