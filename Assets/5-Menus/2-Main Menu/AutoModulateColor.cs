using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoModulateColor : MonoBehaviour
{
    [SerializeField] ColorController _colorController = null;
    [SerializeField] float _fadeDuration = 4;

    [Header("Color list")]
    [SerializeField] List<Color> _colorList = null;

    WaitForSeconds _waitForSeconds;

    void Start()
    {
        _waitForSeconds = new WaitForSeconds(_fadeDuration);
        StartCoroutine(CycleColors());
    }

    IEnumerator CycleColors()
    {
        int i = 0;
        int N = _colorList.Count;

        while (true)
        {
            _colorController.FadeTo(_colorList[i++ % N], _fadeDuration);
            yield return _waitForSeconds;
        }
    }
}
