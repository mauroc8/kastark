using UnityEngine;

[RequireComponent(typeof(AlphaController))]
public class FadeController : MonoBehaviour
{
    AlphaController _alphaController;

    [SerializeField] bool _fadeInOnStart;
    [SerializeField] bool _fadeOutOnStart;
    [SerializeField] float _duration;
    [SerializeField] float _power;
    
    void Awake()
    { _alphaController = GetComponent<AlphaController>(); }

    public void FadeIn()
    {
        _alphaController.FadeIn(_duration, _power);
    }

    public void FadeOut()
    {
        _alphaController.FadeOut(_duration, _power);
    }

    void Start()
    {
        if (_fadeInOnStart != _fadeOutOnStart)
        {
            if (_fadeInOnStart)
                FadeIn();
            else FadeOut();
        }
    }
}