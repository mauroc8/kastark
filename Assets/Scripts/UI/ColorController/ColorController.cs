using UnityEngine;

public abstract class ColorController : AlphaController
{
    public abstract void ChangeColor(Color color);

    public abstract Color GetColor();

    public override void ChangeAlpha(float alpha)
    {
        var color = GetColor();
        color.a = alpha;
        ChangeColor(color);
    }

    public override float GetAlpha()
    {
        return GetColor().a;
    }
}

