using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAndEnableBehaviours : MonoBehaviour
{
    [SerializeField] float _waitSeconds = 0;
    [SerializeField] MonoBehaviour[] _components = new MonoBehaviour[0];
    [SerializeField] bool _autoDisable = true;
    
    void OnEnable()
    {
        StartCoroutine(WaitAndEnable());
    }

    IEnumerator WaitAndEnable()
    {
        yield return new WaitForSeconds(_waitSeconds);
        foreach (MonoBehaviour component in _components)
        {
            component.enabled = true;
        }
        
        if (_autoDisable) enabled = false;
    }
}
