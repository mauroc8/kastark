using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HighlightCharacterOnHover : MonoBehaviour
{
    Color _startcolor;
    Material _material;

    void Start() {
        _material = GetComponent<Renderer>().material;
        _startcolor = _material.color;
    }

    void OnMouseEnter() {
        _material.color = Color.red;
    }

    void OnMouseExit() {
        _material.color = _startcolor;
    }
}
