using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionInFrontOfCreature : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] BodyPart _bodyPart = BodyPart.Chest;
    [SerializeField] float _zOffset = 0;

    [Header("Creature")]
    [SerializeField] bool _useRandomEnemy = false;
    [Tooltip("Ignored if Use Random Enemy is true.")]
    [SerializeField] CreatureController _creature = null;
    
    void Start()
    {
        if (_useRandomEnemy) foreach (var creature in GameState.creaturesInBattle)
        {
            if (GameState.IsFromEnemyTeam(creature))
            {
                _creature = creature;
                break;
            }
        }

        var position = _creature.GetBodyPart(_bodyPart).position;
        position -= Camera.main.transform.forward * _zOffset;
        
        transform.position = position;
    }

}
