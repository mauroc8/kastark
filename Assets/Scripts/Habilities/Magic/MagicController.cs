using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicController : HabilityController
{
    [Header("Config")]
    [SerializeField] float _castDistanceVH = 0.2f;

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
        var followCanvasObject = _particleSystem.GetComponent<FollowCanvasObject>();
        followCanvasObject.depthReference = GameState.actingCreature.gameObject.GetComponent<CreatureTransforms>().head;
        followCanvasObject.enabled = true;

        _bigParticleColor = _bigParticle.GetComponent<ColorController>();
        _particleSystemColor = _particleSystem.GetComponent<ColorController>();

        _bigParticleTransform = _bigParticle.GetComponent<RectTransform>();

        _castDistancePX = _castDistanceVH * Camera.main.pixelHeight;
    }

    bool _casting = false;

    void Update()
    {
        if (_cast) return;

        if (!_casting)
        {
            var diff = Vector2.Distance(_bigParticleTransform.position, Input.mousePosition);

            if (diff <= _castDistancePX)
            {
                _cursorSkin.ChangeCursorTexture(CursorTexture.Interactable);

                if (Input.GetMouseButton(0))
                {
                    _bigParticleColor.ChangeColor(_activeColor);
                    _particleSystemColor.ChangeColor(_activeColor);
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
    }
}
