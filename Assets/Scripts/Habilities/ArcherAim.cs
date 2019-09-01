using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAim : MonoBehaviour
{
    private Transform _background;
    private Transform _arrow;

    private float startTime;

    public float aimSpeed = 1.0f;
    public float aimPosition; // Normalized [-1, 1] where 0 is the best shot.

    public bool paused = false;

    void Start()
    {
        _background = transform.GetChild(0);
        _arrow = transform.GetChild(1);
        startTime = Time.fixedTime;
    }
    void Update()
    {
        if (paused) return;

        if (Input.GetMouseButtonDown(0)) {
            paused = true;
            // Send EVENT to the world! Somebody will catch it.
            // Temporary, the aimPosition is publicly available.
            
            return;
        }

        var arrowPos = _arrow.localPosition;

        var deltaTime = Time.fixedTime - startTime;
        var phase = deltaTime * aimSpeed;
        var phaseI = Mathf.Floor(phase);
        var phaseF = phase - phaseI;

        if (phaseI % 2 == 0) {
            arrowPos.x = -300 + 600 * phaseF;
        } else {
            arrowPos.x = 300 - 600 * phaseF;
        }

        aimPosition = arrowPos.x / 300;

        _arrow.localPosition = arrowPos; 
    }

    
}
