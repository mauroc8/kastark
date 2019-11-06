using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MagicController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float _countdownTime = 0.8f;

    [Header("Refs")]
    [SerializeField] MagicEnergy _magicEnergy;
    [SerializeField] CountdownController _countdown;

    [Header("Position near creature's head")]
    [SerializeField] Creature _creature;
    [SerializeField] Vector2 _offsetVh;

    [Header("Event")]
    [SerializeField] UnityEvent _castEndEvent;

    bool _cast;
    public bool Cast => _cast;

    float _castStartTime;
    Vector3 _lastPosition;

    void OnEnable()
    {
        _cast = false;
        _castStartTime = Time.time;
        _countdown.StartCountdown(_countdownTime);
        _magicEnergy.Open(
            Camera.main.WorldToScreenPoint(_creature.head.position) +
            new Vector3(_offsetVh.x * Screen.height, _offsetVh.y * Screen.height, 0)
        );
    }

    void Update()
    {
        if (_cast) return;

        if (!_countdown.IsRunning)
        {
            _cast = true;
            _castEndEvent.Invoke();
        }

        _magicEnergy.Move(Input.mousePosition);
    }
}
