using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class UI_BattleStartFadeIn : MonoBehaviour
{
    [SerializeField] float _fadeInDuration = 0;
    [SerializeField] float _fadePower      = 0;

    CanvasGroup _canvasGroup;
    float _fadeInStart;

    void OnEnable() {
        EventController.AddListener<BattleStartEvent>(OnBattleStart);
    }
    void OnDisable() {
        EventController.RemoveListener<BattleStartEvent>(OnBattleStart);
    }

    void OnBattleStart(BattleStartEvent e)
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _fadeInStart = Time.time;

        _animating = true;
    }

    bool _animating = false;

    void Update()
    {
        if (_animating) {
            var t = (Time.time - _fadeInStart) / _fadeInDuration;
            if (t < 1) {
                t = Mathf.Pow(t, _fadePower);
                _canvasGroup.alpha = t;
            } else {
                _canvasGroup.alpha = 1;
                this.enabled = false;
            }
        }
    }
}
