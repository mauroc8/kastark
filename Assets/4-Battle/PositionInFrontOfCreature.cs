using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionInFrontOfCreature : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] BodyPart _bodyPart = BodyPart.Chest;
    [Tooltip("Relative to -Camera.forward")]
    [SerializeField] Vector3 _offset = Vector3.zero;
    [SerializeField] bool _offsetRelativeToCamera = true;

    [Header("Creature")]
    [SerializeField] bool _useRandomEnemy = false;
    [SerializeField] bool _useActingCreature = false;
    [Tooltip("Ignored if some of the above is true.")]
    [SerializeField] CreatureController _creature = null;
    
    void Start()
    {
        if (_useRandomEnemy) foreach (var creature in Global.creaturesInBattle)
        {
            if (Global.IsFromEnemyTeam(creature))
            {
                _creature = creature;
                break;
            }
        }
        else if (_useActingCreature)
        {
            _creature = Global.actingCreature;
        }

        var position = _creature.GetBodyPart(_bodyPart).position;

        if (_offsetRelativeToCamera)
        {
            position += Camera.main.transform.forward * _offset.z;
            position += Camera.main.transform.right * _offset.x;
            position += Camera.main.transform.up * _offset.y;
        }
        else
        {
            position += _offset;
        }
        
        transform.position = position;
    }

}
