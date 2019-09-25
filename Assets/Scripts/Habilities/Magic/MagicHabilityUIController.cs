using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHabilityUIController : MonoBehaviour
{
    [SerializeField] MagicController _magicController     = null;

    [Header("Cursor")]
    [SerializeField] CursorSkin _cursorSkin = null;

    [Header("Color controllers")]
    [SerializeField] ColorController _bigParticleColorController    = null;
    [SerializeField] ColorController _particleSystemColorController = null;

    [Header("Fade Out controllers")]
    [SerializeField] ParticleSystem  _particleSystem = null;
    [SerializeField] FadeOutController _bigParticleFadeOutController = null;
    
    [Header("Countdown")]
    [SerializeField] UIRectCountdown _uiRectCountdown = null;

    [Header("Colors prior to casting")]
    [SerializeField] Color _blurColor = Color.gray;
    [SerializeField] Color _hoverColor = Color.white;
    [Header("Colors while casting")]
    [SerializeField] Color _primaryColor = Color.yellow;
    [SerializeField] Color _secondaryColor = Color.yellow;

    bool _lastMouseIsWithinCastDistance;
    bool _lastCasting;

    void Start()
    {
        _cursorSkin.ChangeCursorTexture(CursorTexture.None);
        _bigParticleColorController.ChangeColor(_blurColor);
        _particleSystemColorController.ChangeColor(_blurColor);
    }

    bool _cleaned = false;

    void Update()
    {
        if (_magicController.Cast)
        {
            if (!_cleaned)
            {
                if (_uiRectCountdown.Running) _uiRectCountdown.StopCountdown();

                var emission = _particleSystem.emission;
                emission.enabled = false;

                _bigParticleFadeOutController.FadeOut();

                _cleaned = true;
            }
            return;
        }

        if (_magicController.Casting)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_lastCasting)
                {
                    _cursorSkin.ChangeCursorTexture(CursorTexture.Aggressive);
                    _uiRectCountdown.StartCountdown(_magicController.CountdownTime);

                    var emission = _particleSystem.emission;
                    emission.enabled = true;
                }
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
        _lastCasting = _magicController.Casting;
    }
}
