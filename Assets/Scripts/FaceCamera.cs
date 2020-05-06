using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] bool _updateEachFrame = true;

    void Start()
    {
        transform.LookAt(Camera.main.transform.position);
        enabled = _updateEachFrame;
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
