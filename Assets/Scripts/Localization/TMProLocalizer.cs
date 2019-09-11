using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMProLocalizer : MonoBehaviour
{
    void Start()
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = Localizer.GetLocalizedString(tmp.text);
    }
}
