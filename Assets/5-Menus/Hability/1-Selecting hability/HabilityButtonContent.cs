using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabilityButtonContent : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _name = null;
    [SerializeField] Image _image = null;
    [SerializeField] TextMeshProUGUI _hotkey = null;

    public virtual void FillContent(Hability hability)
    {
        _name.text = hability.LocalizedName;
        _image.sprite = hability.imageSprite;
        if (hability.hotkey != KeyCode.None)
            _hotkey.text = hability.hotkey.ToString();
    }
}