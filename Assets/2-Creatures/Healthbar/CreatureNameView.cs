using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureNameView : MonoBehaviour
{
    [SerializeField] CreatureController _creatureController = null;

    [Header("View Name")]
    [SerializeField] TextMeshProUGUI _nameTMPro = null;

    void Start()
    {
        if (_nameTMPro)
            _nameTMPro.text = _creatureController.creature.creatureName;
    }
}
