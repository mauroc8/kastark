using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHabilityUIController : MonoBehaviour
{
    [SerializeField] MagicController _magicController     = null;

    [Header("Cursor")]
    [SerializeField] CursorSkin _cursorSkin = null;

    [Header("Fade Out controllers")]
    [SerializeField] ParticleSystem  _particleSystem = null;
    [SerializeField] FadeOutController _bigParticleFadeOutController = null;

    void Start()
    {
        _cursorSkin.ChangeCursorTexture(CursorTexture.Aggressive);
    }

    void Update()
    {
        if (_magicController.Cast)
        {
            var emission = _particleSystem.emission;
            emission.enabled = false;

            _bigParticleFadeOutController.FadeOut();
            _cursorSkin.ChangeCursorTexture(CursorTexture.None);

            this.enabled = false;
        }
    }
}
