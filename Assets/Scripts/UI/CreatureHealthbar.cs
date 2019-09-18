using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class CreatureHealthbar : MonoBehaviour
{
    bool _show = false;
    bool _lastShow = true;

    [Header("Settings")]
    [SerializeField] float _imageMarginPercentage = 0;
    [SerializeField] float _distanceToHeadVH = 0; // viewport height (percentage)
    [SerializeField] float _fadeInDuration = 0;
    [SerializeField] float _fadeInPower = 0;

    [Header("Refs")]
    [SerializeField] GameObject _healthbarGroup = null;
    [SerializeField] Transform _headTransform = null;
    [SerializeField] Image _healthImage = null;
    [SerializeField] Image _shieldImage = null;

    CreatureController _creature = null;
    CanvasGroup _canvasGroup;
    Transform _healthbarTransform;
    Vector3 _headPosition;
    float _distanceToHeadPx;

    void Start() {
        _creature = GetComponent<CreatureController>();
        _canvasGroup = _healthbarGroup.GetComponent<CanvasGroup>();
        _headPosition = _headTransform.position;
        _healthbarTransform = _healthbarGroup.transform;
        _distanceToHeadPx = Camera.main.pixelHeight * _distanceToHeadVH;
    }

    void Update() {
        if (_show) {
            Vector2 pos = Camera.main.WorldToScreenPoint(_headPosition);
            pos.y += _distanceToHeadPx;
            
            _healthbarTransform.position = pos;

            if (!_lastShow) {
                _canvasGroup.alpha = 1;
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
                _canvasGroup.alpha = 0;
            }
        }

        if (_fading) {
            var t = (Time.time - _fadeInStart) / _fadeInDuration;

            if (t < 1) {
                t = Mathf.Pow(t, _fadeInPower);
                _canvasGroup.alpha = t;
            } else {
                _canvasGroup.alpha = 1;
                _fading = false;
            }
        }

        _lastShow = _show;
    }

    void OnEnable() {
        EventController.AddListener<BattleStartEvent>(OnBattleStart);
    }
    void OnDisable() {
        EventController.RemoveListener<BattleStartEvent>(OnBattleStart);
    }

    void OnBattleStart(BattleStartEvent e) {
        _show = true;
        FadeIn();
    }

    float _fadeInStart;
    bool _fading;

    void FadeIn() {
        _fadeInStart = Time.time;
        _fading = true;
    }
}
