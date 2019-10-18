using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class HabilityTutorialController : MonoBehaviour
{
    [SerializeField] GameObject _tutorial = null;

    [SerializeField] GameObject[] _disabledGameObjects = null;
    [SerializeField] MonoBehaviour[] _disabledBehaviours = null;

    WaitForSeconds _wait = new WaitForSeconds(0.1f);

    void Start()
    {
        if (Global.selectedHability.timesCast == 0)
        {
            Global.selectedHability.timesCast++;
            StartCoroutine(Tutorial());
        }
    }

    IEnumerator Tutorial()
    {
        foreach (var go in _disabledGameObjects)
        {
            go.SetActive(false);
        }

        foreach (var mb in _disabledBehaviours)
        {
            mb.enabled = false;
        }
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
        DisableTutorial();
    }

    public void DisableTutorial()
    {
        foreach (var go in _disabledGameObjects)
        {
            go.SetActive(true);
        }

        foreach (var mb in _disabledBehaviours)
        {
            mb.enabled = true;
        }
        _tutorial.SetActive(false);
    }
}
