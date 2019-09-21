using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class MagicController : HabilityController
{
    [Header("Config")]
    [SerializeField] float _castDistanceVh = 0.2f;
    [SerializeField] float _fullPowerSpeedVh = 0.017f;

    [Header("Refs")]
    [SerializeField] GameObject _bigParticle = null;
    [SerializeField] GameObject _particleSystem = null;

    [Header("Style")]
    [SerializeField] Color _blurColor = Color.gray;
    [SerializeField] Color _hoverColor = Color.white;
    [SerializeField] Color _activeColor = Color.yellow;
    [SerializeField] CursorSkin _cursorSkin = null;

    ColorController _bigParticleColor;
    ColorController _particleSystemColor;

    RectTransform _bigParticleTransform;
    float _castDistancePX;

    void Start()
    {
        _bigParticleColor = _bigParticle.GetComponent<ColorController>();
        _particleSystemColor = _particleSystem.GetComponent<ColorController>();

        _bigParticleTransform = _bigParticle.GetComponent<RectTransform>();

        _castDistancePX = _castDistanceVh * Camera.main.pixelHeight;
    }

    bool _casting = false;

    void Update()
    {
        if (_cast) return;

        var diff = Vector2.Distance(_bigParticleTransform.position, Input.mousePosition);

        if (!_casting)
        {

            if (diff <= _castDistancePX)
            {
                _cursorSkin.ChangeCursorTexture(CursorTexture.Interactable);

                if (Input.GetMouseButton(0))
                {
                    _casting = true;
                }
                else
                {
                    _bigParticleColor.ChangeColor(_hoverColor);
                    _particleSystemColor.ChangeColor(_hoverColor);
                }
            }
            else
            {
                _cursorSkin.ChangeCursorTexture(CursorTexture.None);

                _bigParticleColor.ChangeColor(_blurColor);
                _particleSystemColor.ChangeColor(_blurColor);
            }
            return;
        }

        // Do something while casting...

        if (Input.GetMouseButton(0))
        {
            var color = Color.Lerp(_activeColor, _hoverColor, diff / _castDistancePX);
            _bigParticleColor.ChangeColor(color);
            _particleSystemColor.ChangeColor(color);
            _cursorSkin.ChangeCursorTexture(CursorTexture.Aggressive);

        }
        else
        {
            _bigParticleColor.ChangeColor(_hoverColor);
            _particleSystemColor.ChangeColor(_hoverColor);
            _cursorSkin.ChangeCursorTexture(CursorTexture.Interactable);
        }

        var target = Util.GetGameObjectAtScreenPoint(_bigParticleTransform.position);
        if (GameState.IsFromEnemyTeam(target))
        {
            var speed = _bigParticle.GetComponent<FollowCursor>().Speed;
            var magnitude = speed.magnitude / Camera.main.pixelHeight / _fullPowerSpeedVh;

            var effectiveness = magnitude > 1 ? 1 : Mathf.Pow(magnitude, difficulty);

            EventController.TriggerEvent(
                new HabilityCastEvent(target.GetComponent<CreatureController>(), effectiveness));
        }
    }
}
