using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ListExtensions;

public class CreatureColorController : StreamBehaviour
{
    [SerializeField] MultiColorController _creatureColorController = null;

    [Header("Colors")]
    [SerializeField] Color _healColor = Color.green;
    [SerializeField] Color _shieldColor = Color.gray;
    [SerializeField] Color _damageColor = Color.red;
    [SerializeField] Color _deadColor = Color.black;

    protected override void Awake()
    {
        var creature = GetComponentInParent<Creature>();

        creature.Events
            .FilterMap(Optional.FromCast<CreatureEvt, CreatureEvts.ReceivedDamage>)
            .Get(_ =>
            {
                _creatureColorController.FadeAndReturn(_damageColor, 0.1f, 0.3f);
            });

        creature.Events
            .FilterMap(Optional.FromCast<CreatureEvt, CreatureEvts.ReceivedHeal>)
            .Get(_ =>
            {
                _creatureColorController.FadeAndReturn(_healColor, 0.1f, 0.3f);
            });
    }

    public void ReceiveShield(float shield)
    {
        _creatureColorController.FadeAndReturn(_shieldColor, 0.1f, 0.3f);
    }
}
