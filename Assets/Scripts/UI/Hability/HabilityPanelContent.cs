using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabilityPanelContent : MonoBehaviour
{
    [SerializeField] Hability[] _habilities = {};
    [SerializeField] protected GameObject _prefab     = null;

    protected virtual void Start()
    {
        for (int i = _habilities.Length - 1; i >= 0; i--)
        {
            var instance = Instantiate(_prefab);
            instance.transform.SetParent(transform, false);
            PositionInstance(instance, i);

            var hability = _habilities[i];
            instance.GetComponent<HabilityButtonContent>()?.FillContent(hability);
            instance.GetComponent<HabilityButtonOnClick>()?.SetHandler(hability);
        }
    }

    protected void PositionInstance(GameObject instance, int i)
    {
        var pos = instance.transform.localPosition;

        var width = GetComponent<RectTransform>().rect.width;
        var buttonWidth = instance.GetComponent<RectTransform>().rect.width;

        var bound = width/2;

        pos.x = Mathf.Lerp(-bound, bound, (float) i / _habilities.Length);

        instance.transform.localPosition = pos;
    }
}
