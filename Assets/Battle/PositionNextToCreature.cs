using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionNextToCreature : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] Creature _creature;
    [SerializeField] BodyPart _bodyPart = BodyPart.Head;

    [Header("Offset")]
    [SerializeField] float _xOffsetVH = 0;
    [SerializeField] float _yOffsetVH = 0;

    void OnEnable()
    {
        var reference = _bodyPart == BodyPart.Head ? _creature.head : _creature.feet;
        Vector2 position = Camera.main.WorldToScreenPoint(reference.position);

        position.x += Screen.height * _xOffsetVH;
        position.y += Screen.height * _yOffsetVH;

        transform.position = position;
    }
}
