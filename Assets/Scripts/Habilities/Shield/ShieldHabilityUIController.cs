using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHabilityUIController : MonoBehaviour
{
    [SerializeField] ShieldController2D _shieldController = null;

    [Header("Collor controllers")]
    [SerializeField] ColorController _backgroundColor = null;
    [SerializeField] ColorController     _circleColor = null;

    [Header("Settings")]
    [SerializeField] float _changeCircleColorThreshold = 0.9f;
    [SerializeField] float _colorBlendThreshold = 0.08f;

    [Header("Colors")]
    [SerializeField] Color _defaultCircleColor = Color.white;
    [SerializeField] Color _alternativeCircleColor = Color.yellow;

    [Header("Cursor")]
    [SerializeField] CursorSkin _cursorSkin = null;

    bool _lastHovering;

    void Update()
    {
        // Color.
        _backgroundColor.ChangeAlpha(_shieldController.Effectiveness);

        if (_shieldController.Effectiveness >= _changeCircleColorThreshold)
            _circleColor.ChangeColor(_alternativeCircleColor);
        else
        {
            var diff = _changeCircleColorThreshold - _shieldController.Effectiveness;
            Color color;
            if (diff <= _colorBlendThreshold)
                color = Color.Lerp(_alternativeCircleColor, _defaultCircleColor, diff / _colorBlendThreshold);
            else
                _circleColor.ChangeColor(_defaultCircleColor);
        }
        
        _circleColor.ChangeAlpha(_shieldController.Effectiveness);

        // Cursor.
        if (_shieldController.Hovering)
        {
            if (!_lastHovering)
            {
                _cursorSkin.ChangeCursorTexture(CursorTexture.Friendly);
            }
        }
        else
        {
            if (_lastHovering)
            {
                _cursorSkin.ChangeCursorTexture(CursorTexture.None);
            }
        }

        _lastHovering = _shieldController.Hovering;
    }
}
