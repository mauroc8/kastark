using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureNameView : MonoBehaviour
{
    [SerializeField] Creature _creature;
    [SerializeField] TextMeshProUGUI _nameText;

    void Start()
    {
        _nameText.text = _creature.creatureName;
    }
}
