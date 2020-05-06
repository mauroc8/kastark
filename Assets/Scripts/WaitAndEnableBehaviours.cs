using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAndEnableBehaviours : MonoBehaviour
{
    [SerializeField] float _waitSeconds;
    [SerializeField] MonoBehaviour[] _components;

    void OnEnable()
    {
        StartCoroutine(WaitAndEnable());
    }

    IEnumerator WaitAndEnable()
    {
        yield return new WaitForSeconds(_waitSeconds);
        foreach (MonoBehaviour component in _components)
            component.enabled = true;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        foreach (MonoBehaviour component in _components)
            component.enabled = false;
    }
}
