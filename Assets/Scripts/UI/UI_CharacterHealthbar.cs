using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterHealthbar : MonoBehaviour
{
    [SerializeField]
    CharacterStats _characterAttributes = null;

    [SerializeField] Texture2D _backgroundTexture = null;
    [SerializeField] Texture2D _healthTexture = null;
    [SerializeField] Texture2D _bigMarkerTexture = null;
    [SerializeField] Texture2D _smallMarkerTexture = null;

    float _distanceToHead = 60;

    float _leftMargin = 3; // Amount of transparent pixels to the left in _healthTexture
    float _rightMargin = 4;

    float _backgroundTextureWidth;
    float _backgroundTextureHeight;

    void Start() {
        _backgroundTextureWidth = _backgroundTexture.width;
        _backgroundTextureHeight = _backgroundTexture.height;
        
        //CalculateScreenPosition();
    }

    Vector2 _screenPosition;
    
    void CalculateScreenPosition() {
        _screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        _screenPosition.x = _screenPosition.x - _backgroundTextureWidth / 2;
        _screenPosition.y = Camera.main.pixelHeight - _screenPosition.y;

        _screenPosition.y -= _distanceToHead;
    }

    float _healthTextureWidth;
    float _healthPointWidth;

    void CalculateTexturesWidth() {
        float healthPercentage = _characterAttributes.health / _characterAttributes.maxHealth;
        float margins = _leftMargin + _rightMargin;
        float healthTextureMaxWidth = _backgroundTextureWidth - margins;

        _healthTextureWidth = _leftMargin + healthTextureMaxWidth * healthPercentage;

        _healthPointWidth = healthTextureMaxWidth / _characterAttributes.maxHealth;
    }

    void OnGUI() {
        CalculateScreenPosition();
        CalculateTexturesWidth();

        GUI.DrawTexture(
            new Rect(_screenPosition.x, _screenPosition.y, _backgroundTextureWidth, _backgroundTextureHeight),
            _backgroundTexture
        );

        GUI.DrawTextureWithTexCoords(
            new Rect(_screenPosition.x, _screenPosition.y,
                _healthTextureWidth,
                _backgroundTextureHeight),
            _healthTexture,
            new Rect(0, 0, _healthTextureWidth / _backgroundTextureWidth, 1),
            true
        );

        /*
        for (int i = 0; i <= _characterAttributes.health; i++) {
            float x = _screenPosition.x + _leftMargin + i * _healthPointWidth;
            Texture2D texture = i % 10 == 0 ? _bigMarkerTexture : _smallMarkerTexture;
            GUI.DrawTexture(
                new Rect(x, _screenPosition.y, texture.width, texture.height),
                texture
            );
        }
         */
    }
}
