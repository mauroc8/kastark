using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeExtensions;


public abstract class StreamBehaviour<A> : MonoBehaviour where A : new()
{
    protected abstract void Awake();

    /* --- Lifecycle streams -- */

    protected Stream<A> enableStream => _enableSource;
    protected Stream<A> startStream => _startSource;
    protected Stream<A> disableStream => _disableSource;
    public Stream<A> destroyStream => _destroySource;

    private bool _usedUpdate = false;
    protected Stream<A> updateStream
    {
        get
        {
            if (!_usedUpdate)
            {
                enableStream.Get(_ =>
                {
                    StartCoroutine(UpdateCoroutine());
                });

                _usedUpdate = true;
            }
            return _updateSource;
        }
    }

    StreamSource<A> _enableSource = new StreamSource<A>();
    StreamSource<A> _startSource = new StreamSource<A>();
    StreamSource<A> _updateSource = new StreamSource<A>();
    StreamSource<A> _disableSource = new StreamSource<A>();
    StreamSource<A> _destroySource = new StreamSource<A>();

    void OnEnable()
    {
        _enableSource.Push(new A());
    }

    void Start()
    {
        _startSource.Push(new A());
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            yield return null;
            _updateSource.Push(new A());
        }
    }

    void OnDisable()
    {
        _disableSource.Push(new A());
    }

    protected virtual void OnDestroy()
    {
        _destroySource.Push(new A());

        _enableSource.Destroy();
        _startSource.Destroy();
        _updateSource.Destroy();
        _disableSource.Destroy();
        _destroySource.Destroy();
    }
}

