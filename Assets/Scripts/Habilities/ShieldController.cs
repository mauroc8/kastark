using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class ShieldController : MonoBehaviour
{
    [SerializeField] RectTransform _circleTransform = null;
    [SerializeField] float _speed = 3;

    void OnEnable() {
        var unitPos = GameState.currentUnit.transform.position;
        var cameraPos = Camera.main.transform.position;

        transform.position = Vector3.MoveTowards(unitPos, cameraPos, 3);
        transform.LookAt(Camera.main.transform.position, Vector3.up);
    }

    bool _cast = false;
    float _effectiveness;

    void Update() {
        if (_cast) return;

        var time = Time.time;

        var cycle = Mathf.Sin(time * _speed);
        var scale = 1 + cycle * 0.5f;

        _circleTransform.localScale = new Vector3(scale, scale, 1);

        if (Input.GetMouseButtonDown(0) && !Util.MouseIsOnUI()) {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)) {
                var target = hit.transform.gameObject;
                if (target.CompareTag(GameState.PlayerTeamTag)) {
                    _cast = true;
                    _effectiveness = Mathf.Abs(1 - scale);
                    EventController.TriggerEvent(new ConfirmSelectedHabilityEvent());
                }
            }
        }
    }
}
