using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiColorController : ColorController
{
    [SerializeField] ColorController[] _colorControllers = null;

    public override void ChangeColor(Color color)
    {
        for (int i = 0; i < _colorControllers.Length; i++)
        {
            _colorControllers[i].ChangeColor(color);
        }
    }

    public void ChangeColors(Color[] colors)
    {
        for (int i = 0; i < _colorControllers.Length && i < colors.Length; i++)
        {
            _colorControllers[i].ChangeColor(colors[i]);
        }
    }

    public override Color GetColor()
    {
        return _colorControllers[0].GetColor();
    }

    public Color[] GetColors()
    {
        var colors = new Color[_colorControllers.Length];

        for (int i = 0; i < _colorControllers.Length; i++)
        {
            colors[i] = _colorControllers[i].GetColor();
        }

        return colors;
    }
}
