using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart
{
    Head, Chest, Feet, None
};

public class PositionNextToCreature : MonoBehaviour
{
    [Header("Creature")]
    [Tooltip("If null, the current active creature will be used.")]
    [SerializeField] CreatureController _creature = null;
    [SerializeField] BodyPart           _bodyPart = BodyPart.Chest;

    [Header("Offset")]
    [SerializeField] float _xOffsetVH = 0;
    [SerializeField] float _yOffsetVH = 0;

    [Header("Settings")]
    [SerializeField] bool _negateXOffsetToFaceEnemy = false;

    float _xOffsetPx;
    float _yOffsetPx;

    void OnEnable()
    {
        if (_bodyPart == BodyPart.None) return;

        var creatureController = _creature == null ? GameState.actingCreature : _creature;

        var reference = _bodyPart == BodyPart.Head  ? creatureController.head  :
                        _bodyPart == BodyPart.Chest ? creatureController.chest :
                                                      creatureController.feet  ;

        _xOffsetPx = Camera.main.pixelHeight * _xOffsetVH;
        _yOffsetPx = Camera.main.pixelHeight * _yOffsetVH;

        if (_negateXOffsetToFaceEnemy && GameState.GetTeamOf(creatureController) == Team.Right)
        {
            _xOffsetPx *= -1;
        }

        Vector2 position = Camera.main.WorldToScreenPoint(reference.position); 

        position.x += _xOffsetPx;
        position.y += _yOffsetPx;

        transform.position = position;
    }
}
