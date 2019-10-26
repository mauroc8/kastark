using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class HabilityTutorialController : MonoBehaviour
{
    [SerializeField] List<GameObject> _enableOnStart = null;
    [SerializeField] List<GameObject> _disableOnStart = null;
    
    int _timesCast = 0;

    void Start()
    {
        if (_timesCast == 0)
        {
            _disableOnStart.ForEach(go => go.SetActive(false));
            _enableOnStart.ForEach(go => go.SetActive(true));
        }
        _timesCast++;
    }

    public void EndTutorial()
    {
        _disableOnStart.ForEach(go => go.SetActive(true));
        _enableOnStart.ForEach(go => go.SetActive(false));
    }
}
