using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class CreatureHealthbar : MonoBehaviour
{
    bool _lastShow = true;

    float _imageMarginPercentage = 0.0033f;
    float _distanceToHeadVH = 0.08f;

    [Header("Refs")]
    [SerializeField] CanvasGroup _healthbarGroup = null;
    [SerializeField] Image _healthImage = null;
    [SerializeField] Image _shieldImage = null;

    [Header("Settings")]
    [SerializeField] bool _show = true;

    Creature _creature;
    Transform _healthbarTransform;
    Transform _headTransform;
    float _distanceToHeadPx;

    void Start() {
        _healthbarTransform = _healthbarGroup.transform;
        
        var creatureController = GetComponent<CreatureController>();
        _creature = creatureController.creature;
        
        _headTransform = creatureController.head;

        _distanceToHeadPx = Screen.height * _distanceToHeadVH;
    }

    void Update() {
        if (_show) {
            Vector2 pos = Camera.main.WorldToScreenPoint(_headTransform.position);
            pos.y += _distanceToHeadPx;
            
            _healthbarTransform.position = pos;

            if (!_lastShow) {
                _healthbarGroup.alpha = 1;
            }

            var health = _creature.health;
            var shield = _creature.shield;
            var maxHealth = _creature.maxHealth;

            var max = Mathf.Max(health + shield, maxHealth);
            var healthPercent = health / max;
            var shieldPercent = (health + shield) / max;

            var healthFillAmount = _imageMarginPercentage + healthPercent * (1 - 2 * _imageMarginPercentage);
            var shieldFillAmount = _imageMarginPercentage + shieldPercent * (1 - 2 * _imageMarginPercentage);

            _healthImage.fillAmount = healthFillAmount;
            _shieldImage.fillAmount = shieldFillAmount;

        } else {
            if (_lastShow) {
                _healthbarGroup.alpha = 0;
            }
        }

        _lastShow = _show;
    }
}
