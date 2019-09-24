using UnityEngine;

public abstract class ColorController : AlphaController
{
    public virtual void ChangeColor(Color color)
    {
        Debug.Log("Not Implemented.");
    }
    public virtual Color GetColor()
    {
        Debug.Log("Not Implemented.");
        return Color.white;
    }

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

