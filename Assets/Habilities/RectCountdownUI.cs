using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectCountdownUI : MonoBehaviour
{
    [SerializeField] CountdownController _countdownController;
    [SerializeField] RectTransform _rectTransform;

    void Update()
    {
        if (_countdownController.IsRunning)
        {
            _rectTransform.localScale = new Vector3(_countdownController.Progress, 1, 1);
        }
    }
}
