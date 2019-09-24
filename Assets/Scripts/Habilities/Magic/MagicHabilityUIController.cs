using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHabilityUIController : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] CursorSkin _cursorSkin = null;

    [Header("Refs")]
    [SerializeField] MagicController _magicController     = null;
    [SerializeField] ColorController _bigParticleColorController    = null;
    [SerializeField] ColorController _particleSystemColorController = null;

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
        _bigParticleColorController.ChangeColor(_blurColor);
        _particleSystemColorController.ChangeColor(_blurColor);
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
                _bigParticleColorController.ChangeColor(color);
                _particleSystemColorController.ChangeColor(color);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _cursorSkin.ChangeCursorTexture(CursorTexture.Interactable);

                _bigParticleColorController.ChangeColor(_secondaryColor);
                _particleSystemColorController.ChangeColor(_secondaryColor);
            }
        }
        else
        {
            if (_magicController.MouseIsWithinCastDistance)
            {
                if (!_lastMouseIsWithinCastDistance)
                {
                    _cursorSkin.ChangeCursorTexture(CursorTexture.Interactable);

                    _bigParticleColorController.ChangeColor(_hoverColor);
                    _particleSystemColorController.ChangeColor(_hoverColor);
                }
            }
            else
            {
                if (_lastMouseIsWithinCastDistance)
                {
                    _cursorSkin.ChangeCursorTexture(CursorTexture.None);

                    _bigParticleColorController.ChangeColor(_blurColor);
                    _particleSystemColorController.ChangeColor(_blurColor);
                }
            }
        }

        _lastMouseIsWithinCastDistance = _magicController.MouseIsWithinCastDistance;
    }
}
