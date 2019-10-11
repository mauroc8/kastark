using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointMovementController : MonoBehaviour
{
    [SerializeField] float _movementSpeed = 4;

    public void MoveTowards(Vector3 target)
    {
        var pos = transform.localPosition;
        transform.localPosition = Vector3.MoveTowards(pos, target, _movementSpeed * Time.deltaTime);
    }
}
