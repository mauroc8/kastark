using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HabilityButtonContent : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text = null;
    [SerializeField] Image _image = null;

    public void FillContent(Hability hability)
    {
        _text.text = hability.Name;
        _image.sprite = hability.ImageSprite;
    }
}