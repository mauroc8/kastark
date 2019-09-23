using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSmoothnessController : MonoBehaviour
{
    Material _material;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }
    
    public void ChangeSmoothness(float smoothness)
    {
        _material.SetFloat("_Glossiness", smoothness);
    }
}
