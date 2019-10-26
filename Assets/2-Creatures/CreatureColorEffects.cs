using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class CreatureColorEffects : MonoBehaviour
{
    [SerializeField] MultiColorController _creatureColorController = null;

    [Header("Colors")]
    [SerializeField] Color _healColor = Color.green;
    [SerializeField] Color _shieldColor = Color.gray;
    [SerializeField] Color _damageColor = Color.red;
    [SerializeField] Color _deadColor   = Color.black;

    public void ReceiveDamage(float damage)
    {
        _creatureColorController.FadeAndReturn(_damageColor, 0.1f, 0.3f);
    }

    public void ReceiveShield(float shield)
    {
        _creatureColorController.FadeAndReturn(_shieldColor, 0.1f, 0.3f);
    }

    public void ReceiveHeal(float heal)
    {
        _creatureColorController.FadeAndReturn(_healColor, 0.1f, 0.3f);
    }
}
