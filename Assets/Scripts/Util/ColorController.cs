using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorController : MonoBehaviour
{
    public void ChangeOpacity(float a)
    {
        var renderer = GetComponent<Renderer>();
        if (renderer)
        {
            var color = renderer.material.color;
            color.a = a;
            renderer.material.color = color;
            return;
        }
        
        var group = GetComponent<CanvasGroup>();
        if (group)
        {
            group.alpha = a;
            return;
        }
        var image = GetComponent<Image>();
        if (image)
        {
            var color = image.color;
            color.a = a;
            image.color = color;
            return;
        }

        throw new MissingComponentException($"Expected a Renderer, CanvasGroup or Image component in {name}.");
    }

    public void ChangeColor(Color color)
    {
        var renderer = GetComponent<Renderer>();
        if (renderer)
        {
            renderer.material.color = color;
        }
        var image = GetComponent<Image>();
        if (image)
        {
            image.color = color;
        }
    }

    public Color GetColor()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer)
        {
            return renderer.material.color;
        }
        var image = GetComponent<Image>();
        if (image)
        {
            return image.color;
        }
        
        throw new MissingComponentException($"Expected a Renderer or Image component in {name}.");
    }
}
