using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Kastark/ItemsDictionary")]
public class ItemsDictionary : ScriptableObject
{
    [Header("Icons")]

    public Sprite empty = null;

    public Sprite smallSword = null;
    public Sprite bigSword = null;

    public Sprite smallMagic = null;
    public Sprite bigMagic = null;

    public Sprite shield = null;

    public Sprite potion = null;

    public Sprite clover = null;


    public Sprite SpriteFromItem(Item item)
    {
        switch (item)
        {
            case Items.Empty _:
                return empty;

            case Items.Sword _:
                return bigSword;

            case Items.Magic _:
                return bigMagic;

            case Items.Shield _:
                return shield;

            case Items.Potion _:
                return potion;

            case Items.SmallSword _:
                return smallSword;

            case Items.SmallMagic _:
                return smallMagic;

            case Items.Clover _:
                return clover;

            default:
                return null;
        }
    }
}
