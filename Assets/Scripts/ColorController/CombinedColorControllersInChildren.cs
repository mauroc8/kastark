using System.Collections.Generic;
using UnityEngine;

public class CombinedColorControllersInChildren : MultiColorController
{
    List<ColorController> _colorControllers;

    void Awake()
    {
        _colorControllers = new List<ColorController>(
            GetComponentsInChildren<ColorController>()
        );
    }

    public override List<Color> MyColors
    {
        get { return _colorControllers.ConvertAll(colorController => colorController.MyColor); }
        set {
            Debug.Assert(_colorControllers.Count == value.Count);

            for (int i = 0; i < value.Count; i++)
                _colorControllers[i].MyColor = value[i];
        }
    }
}
