using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class CreatureUIController : MonoBehaviour
{
    [SerializeField] ColorController     _creatureColorController     = null;
    [SerializeField] ColorFadeController _creatureColorFadeController = null;

    [Header("Colors")]
    [SerializeField] Color _healColor = Color.green;
    [SerializeField] Color _shieldColor = Color.gray;
    [SerializeField] Color _damageColor = Color.red;

    Color _defaultColor;

    void OnEnable()
    {
        _defaultColor = _creatureColorController.GetColor();
    }

    public void ReceiveDamage(float damage)
    {
        ChangeColorAndFadeBack(_damageColor);
    }

    public void ReceiveShield(float shield)
    {
        ChangeColorAndFadeBack(_shieldColor);
    }

    public void ReceiveHeal(float heal)
    {
        ChangeColorAndFadeBack(_healColor);
    }
    
    void ChangeColorAndFadeBack(Color damageColor)
    {
        _creatureColorController.ChangeColor(damageColor);
        _creatureColorFadeController.FadeTo(_defaultColor);
    }
}
