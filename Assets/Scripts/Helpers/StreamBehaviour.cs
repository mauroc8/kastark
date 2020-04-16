using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that has only one possible value.
/// A `new Void()` represents nothing, or anything we don't care about.
/// It's used in `EventStreams` when they have no relevant payload.
/// </summary>
public struct Void { }


/// <summary>
/// A class that has no values. A `Never` value can never be constructed.
/// Under the hood, it's just an abstract class with no sub-classes.
/// </summary>
public abstract class Never { }

/// <summary>
/// A MonoBehaviour with access to its lifecycle events as streams.
/// Eg.: `enable`, `disable`, `update`.
/// </summary>
public abstract partial class StreamBehaviour : MonoBehaviour
{
    protected abstract void Awake();

    /* --- Lifecycle streams -- */

    protected Stream<Void> enable => _enableSource;
    protected Stream<Void> start => _startSource;
    protected Stream<Void> update => _updateSource;
    protected Stream<Void> disable => _disableSource;
    public Stream<Void> destroy => _destroySource;

    StreamSource<Void> _enableSource = new StreamSource<Void>();
    StreamSource<Void> _startSource = new StreamSource<Void>();
    StreamSource<Void> _updateSource = new StreamSource<Void>();
    StreamSource<Void> _disableSource = new StreamSource<Void>();
    StreamSource<Void> _destroySource = new StreamSource<Void>();

    protected virtual void Start()
    {
        _startSource.Push(new Void());
    }

    void OnEnable()
    {
        _enableSource.Push(new Void());
    }

    void OnDisable()
    {
        _disableSource.Push(new Void());
    }

    protected virtual void OnDestroy()
    {
        _destroySource.Push(new Void());
    }

    void Update()
    {
        _updateSource.Push(new Void());
    }
}

