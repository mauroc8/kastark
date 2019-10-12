using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCursorController : MonoBehaviour
{
    [SerializeField] CursorSkin _cursorSkin = null;

    void OnMouseEnter()
    {
        _cursorSkin.ChangeCursorTexture(CursorTexture.Friendly);
    }

    void OnMouseExit()
    {
        _cursorSkin.ChangeCursorTexture(CursorTexture.None);
    }
}
