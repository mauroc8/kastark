using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAround : MonoBehaviour
{
    [SerializeField] float xSpeed = 1;
    [SerializeField] float ySpeed = 1;
    [SerializeField] float xDisplacement = 30;
    [SerializeField] float yDisplacement = 30;

    void Update()
    {
        var t = Time.time;
        var pos = transform.localPosition;
        pos.x = Mathf.Sin(t * xSpeed) * xDisplacement;
        pos.y = Mathf.Cos(t * ySpeed) * yDisplacement;
        transform.localPosition = pos;
    }
}
