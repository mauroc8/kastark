using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectCountdownUI : MonoBehaviour
{
    [SerializeField] CountdownController _countdownController = null;
    [SerializeField] RectTransform _rectTransform = null;

    void Update()
    {
        if (_countdownController.Running)
        {
            _rectTransform.localScale = new Vector3(_countdownController.Progress, 1, 1);
        }
    }
}
