using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupAlphaController : AlphaController
{
    CanvasGroup _canvasGroup = null;

    void Init()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
    }

    public override float Alpha
    {
        get
        {
            Init();
            return _canvasGroup.alpha;
        }
        set
        {
            Init();
            _canvasGroup.alpha = value;
        }
    }
}
