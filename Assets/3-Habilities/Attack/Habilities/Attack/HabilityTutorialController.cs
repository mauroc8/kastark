using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class HabilityTutorialController : MonoBehaviour
{
    [SerializeField] GameObject _tutorial = null;

    WaitForSeconds _wait;

    void Start()
    {
        if (Global.selectedHability.TimesCast == 0)
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
