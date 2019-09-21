using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHabilityUIController : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] CursorSkin _cursorSkin = null;

    [Header("Refs")]
    [SerializeField] MagicController _magicController     = null;
    [SerializeField] ColorController _bigParticleColor    = null;
    [SerializeField] ColorController _particleSystemColor = null;

    [Header("Colors prior to casting")]
    [SerializeField] Color _blurColor = Color.gray;
    [SerializeField] Color _hoverColor = Color.white;
    [Header("Colors while casting")]
    [SerializeField] Color _primaryColor = Color.yellow;
    [SerializeField] Color _secondaryColor = Color.yellow;

    bool _lastMouseIsWithinCastDistance;

    void Start()
    {
        _cursorSkin.ChangeCursorTexture(CursorTexture.None);
        _bigParticleColor.ChangeColor(_blurColor);
        _particleSystemColor.ChangeColor(_blurColor);
    }

    void Update()
    {
        if (_magicController.Cast) return;

        if (_magicController.Casting)
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _cursorSkin.ChangeCursorTexture(CursorTexture.Aggressive);
                }

                var color = Color.Lerp(_primaryColor, _secondaryColor, _magicController.NormalizedDistanceToAttractionCenter);
                _bigParticleColor.ChangeColor(color);
                _particleSystemColor.ChangeColor(color);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _cursorSkin.ChangeCursorTexture(CursorTexture.Interactable);

                _bigParticleColor.ChangeColor(_secondaryColor);
                _particleSystemColor.ChangeColor(_secondaryColor);
            }
        }
        else
        {
            if (_magicController.MouseIsWithinCastDistance)
            {
                if (!_lastMouseIsWithinCastDistance)
                {
                    _cursorSkin.ChangeCursorTexture(CursorTexture.Interactable);

                    _bigParticleColor.ChangeColor(_hoverColor);
                    _particleSystemColor.ChangeColor(_hoverColor);
                }
            }
            else
            {
                if (_lastMouseIsWithinCastDistance)
                {
                    _cursorSkin.ChangeCursorTexture(CursorTexture.None);

                    _bigParticleColor.ChangeColor(_blurColor);
                    _particleSystemColor.ChangeColor(_blurColor);
                }
            }
        }

        _lastMouseIsWithinCastDistance = _magicController.MouseIsWithinCastDistance;
    }
}
