using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDirection : MonoBehaviour
{
    [Header("Cosmetics")]
    public SpriteRenderer Hair;
    public SpriteRenderer LeftEar;
    public SpriteRenderer RightEar;

    [Header("Equipment")]
    public SpriteRenderer LeftLeg;
    public SpriteRenderer RightLeg;
    public SpriteRenderer RightArm;
    public SpriteRenderer LeftArm;
    public SpriteRenderer Helmet;
    public SpriteRenderer Armor;

    [Header("Right Hand")]
    public SpriteRenderer MainWeapon;

    [Header("Left Hand")]
    public SpriteRenderer Handle;
    public SpriteRenderer LimbU;
    public SpriteRenderer LimbL;
}
