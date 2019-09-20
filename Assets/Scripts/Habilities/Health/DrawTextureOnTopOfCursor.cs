using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawTextureOnTopOfCursor : MonoBehaviour
{
    [SerializeField] Texture _texture = null;
    [SerializeField] int _width;
    [SerializeField] int _height;

    void Start()
    {
        if (_width == 0) _width = _texture.width;
        if (_height == 0) _height = _texture.height;
    }

    void OnGUI()
    {
        var pos = Input.mousePosition;
        pos.y = Camera.main.pixelHeight - pos.y;

        GUI.depth = 0;
        GUI.DrawTexture(new Rect(pos.x, pos.y, _width, _height), _texture);
    }
}
