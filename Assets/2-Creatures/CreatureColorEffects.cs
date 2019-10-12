using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class CreatureColorEffects : MonoBehaviour
{
    [SerializeField] ColorFadeController _creatureColorFadeController = null;

    [Header("Colors")]
    [SerializeField] Color _healColor = Color.green;
    [SerializeField] Color _shieldColor = Color.gray;
    [SerializeField] Color _damageColor = Color.red;
    [SerializeField] Color _deadColor   = Color.black;

    public void ReceiveDamage(float damage)
    {
        _creatureColorFadeController.FadeFrom(_damageColor);
    }

    public void ReceiveShield(float shield)
    {
        _creatureColorFadeController.FadeFrom(_shieldColor);
    }

    public void ReceiveHeal(float heal)
    {
        _creatureColorFadeController.FadeFrom(_healColor);
    }
}
