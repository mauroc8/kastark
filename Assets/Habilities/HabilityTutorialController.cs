﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEvents;

public class HabilityTutorialController : MonoBehaviour
{
    [SerializeField] GameObject _tutorial = null;
    [SerializeField] MonoBehaviour _controller = null;

    //int _timesCast = 0;

    void Awake()
    {
        _tutorial.SetActive(false);
        _controller.enabled = true;
    }

    void OnEnable()
    {
        /*
        if (_timesCast == 0)
            _tutorial.SetActive(true);
        else
            _controller.enabled = true;

        _timesCast++;
        */
    }

    void OnDisable()
    {
        /*
        _tutorial.SetActive(false);
        _controller.enabled = false;
        */
    }

    public void EndTutorial()
    {
        /*
        _tutorial.SetActive(false);
        _controller.enabled = true;
        */
    }
}
