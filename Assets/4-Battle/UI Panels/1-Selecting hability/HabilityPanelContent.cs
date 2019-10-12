using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabilityPanelContent : MonoBehaviour
{
    [SerializeField] protected GameObject _buttonPrefab     = null;

    List<Hability> _habilities;

    protected virtual void OnEnable()
    {
        _habilities = Global.actingCreature.creature.habilities;
        var N = _habilities.Count;

        for (int i = N - 1; i >= 0; i--)
        {
            var instance = Instantiate(_buttonPrefab);
            instance.transform.SetParent(transform, false);
            PositionInstance(instance, i, N);

            var hability = _habilities[i];

            instance.GetComponent<HabilityButtonContent>()?.FillContent(hability);
            instance.GetComponent<HabilityButtonHandler>()?.SetHandler(hability);
        }
    }

    protected void PositionInstance(GameObject instance, int i, int N)
    {
        var pos = instance.transform.localPosition;

        var width = GetComponent<RectTransform>().rect.width;
        var buttonWidth = instance.GetComponent<RectTransform>().rect.width;

        var bound = width/2;

        pos.x = Mathf.Lerp(-bound, bound, (float)i / (float)N);

        instance.transform.localPosition = pos;
    }
}
