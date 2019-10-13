using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoModulateColor : MonoBehaviour
{
    [SerializeField] ColorFadeController _colorFadeController = null;

    [Header("Color list")]
    [SerializeField] List<Color> _colorList = null;

    WaitForSeconds _waitForSeconds;

    void Start()
    {
        _waitForSeconds = new WaitForSeconds(_colorFadeController.FadeDuration);
        StartCoroutine(CycleColors());
    }

    IEnumerator CycleColors()
    {
        int i = 0;
        int N = _colorList.Count;

        while (true)
        {
            _colorFadeController.FadeTo(_colorList[i++ % N]);
            yield return _waitForSeconds;
        }
    }
}
