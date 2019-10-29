using System.Collections.Generic;
using UnityEngine;

public class CombinedAlphaControllersInChildren : MultiAlphaController
{
    List<AlphaController> _alphaControllers;

    void Awake()
    {
        _alphaControllers = new List<AlphaController>(
            GetComponentsInChildren<AlphaController>()
        );
    }

    public override List<float> Alphas
    {
        get { return _alphaControllers.ConvertAll(alphaController => alphaController.Alpha); }
        set {
            Debug.Assert(_alphaControllers.Count == value.Count);

            for (int i = 0; i < value.Count; i++)
                _alphaControllers[i].Alpha = value[i];
        }
    }
}
