using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureUIController : MonoBehaviour
{
    [SerializeField] CreatureController _creatureController = null;
    [SerializeField] TextMeshProUGUI _nameTMPro = null;

    void Start()
    {
        _nameTMPro.text = _creatureController.creature.creatureName;
    }
}
