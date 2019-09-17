using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FollowCursor : MonoBehaviour
{
    [SerializeField] float _attraction = 0.8f;
    
    void Update() {
        Vector2 cursor = Input.mousePosition;

        var attraction = _attraction * Time.deltaTime;
        var pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, cursor.x, attraction);
        pos.y = Mathf.Lerp(pos.y, cursor.y, attraction);
        transform.position = pos;
    }
}
