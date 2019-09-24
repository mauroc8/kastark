using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class AttackUIController : MonoBehaviour
{
    static bool _isFirstTime = true;

    [SerializeField] GameObject _tutorial = null;

    WaitForSeconds _wait;

    void Start()
    {
        if (_isFirstTime)
        {
            _wait = new WaitForSeconds(0.3f);
        }
        else
        {
            _wait = new WaitForSeconds(10);
        }
        StartCoroutine(Tutorial());
    }

    IEnumerator Tutorial()
    {
        yield return _wait;
        _tutorial.SetActive(true);
    }

    void OnEnable()
    {
        EventController.AddListener<HabilityCastEvent>(OnHabilityCast);
    }

    void OnDisable()
    {
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCast);
    }

    void OnHabilityCast(HabilityCastEvent evt)
    {
        _tutorial.SetActive(false);
    }
}
