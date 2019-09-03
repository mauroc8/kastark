using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HabilityDescriptionController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _titleText = null;
    [SerializeField] TextMeshProUGUI _descriptionText = null;

    public void SetDescription(HabilityInfo habilityInfo) {
        _titleText.text = habilityInfo.habilityName;
        _descriptionText.text = habilityInfo.description;
    }
}
