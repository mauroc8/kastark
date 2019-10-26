using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionNextToCreature : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] CreatureController _creature;
    [SerializeField] BodyPart           _bodyPart = BodyPart.Chest;

    [Header("Offset")]
    [SerializeField] float _xOffsetVH = 0;
    [SerializeField] float _yOffsetVH = 0;

    [Header("Settings")]
    [SerializeField] bool _negateXOffsetToFaceEnemy = false;
    [Tooltip("Use true only if the component is in a 2D object.")]
    [SerializeField] bool _useScreenSpace = true;

    float _xOffsetPx;
    float _yOffsetPx;
    Transform _reference;

    void OnEnable()
    {
        if (_bodyPart == BodyPart.None) return;

        if (_creature == null)
        {
            Debug.Log("Position next to creature has no creature attached.");
            return;
        }

        _reference = _bodyPart == BodyPart.Head  ? _creature.head  :
                        _bodyPart == BodyPart.Chest ? _creature.chest :
                                                      _creature.feet  ;

        _xOffsetPx = Screen.height * _xOffsetVH;
        _yOffsetPx = Screen.height * _yOffsetVH;

        if (_negateXOffsetToFaceEnemy && Global.GetTeamOf(_creature) == Team.Right)
        {
            _xOffsetPx *= -1;
        }

        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector2 position = _useScreenSpace ? Camera.main.WorldToScreenPoint(_reference.position) : _reference.position; 

        position.x += _xOffsetPx;
        position.y += _yOffsetPx;

        transform.position = position;
    }
}
