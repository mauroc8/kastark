using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class CreatureUIController : MonoBehaviour
{
    [SerializeField] CreatureController  _creatureController          = null;
    [SerializeField] ColorController     _creatureColorController     = null;
    [SerializeField] ColorFadeController _creatureColorFadeController = null;

    [Header("Colors")]
    [SerializeField] Color _healColor = Color.green;
    [SerializeField] Color _shieldColor = Color.white;
    [SerializeField] Color _damageColor = Color.red;

    Color _defaultColor;

    void OnEnable()
    {
        EventController.AddListener<HabilityCastEvent>(OnHabilityCast);

        _defaultColor = _creatureColorController.GetColor();
    }

    void OnDisable()
    {
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCast);
    }
    
    void OnHabilityCast(HabilityCastEvent evt)
    {
        foreach (var target in evt.targets)
        {
            if (target == _creatureController)
            {
                ChangeColorToDamageType(evt.damageType);
                return;
            }
        }
    }

    void ChangeColorToDamageType(DamageType damageType)
    {
        var damageColor  =
            damageType == DamageType.Heal ? _healColor :
            damageType == DamageType.Shield ? _shieldColor :
            _damageColor;
        _creatureColorController.ChangeColor(damageColor);
        _creatureColorFadeController.FadeTo(_defaultColor);
    }
}
