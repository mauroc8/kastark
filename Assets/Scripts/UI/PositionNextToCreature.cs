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
    public bool updateEachFrame = false;
    [Tooltip("Use true only if the component is in a 2D object.")]
    [SerializeField] bool _useScreenSpace = true;

    float _xOffsetPx;
    float _yOffsetPx;
    Transform _reference;

    void OnEnable()
    {
        if (_bodyPart == BodyPart.None) return;

        var creatureController = _creature == null ? Global.actingCreature : _creature;

        _reference = _bodyPart == BodyPart.Head  ? creatureController.head  :
                        _bodyPart == BodyPart.Chest ? creatureController.chest :
                                                      creatureController.feet  ;

        _xOffsetPx = Screen.height * _xOffsetVH;
        _yOffsetPx = Screen.height * _yOffsetVH;

        if (_negateXOffsetToFaceEnemy && Global.GetTeamOf(creatureController) == Team.Right)
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

    void Update()
    {
        if (updateEachFrame) UpdatePosition();
    }
}
