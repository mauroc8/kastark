using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupAlphaController : AlphaController
{
    CanvasGroup _canvasGroup;

    void Awake()
    { _canvasGroup = GetComponent<CanvasGroup>(); }
    
    public override float Alpha
    {
        get { return _canvasGroup.alpha; }
        set { _canvasGroup.alpha = value; }
    }
}
