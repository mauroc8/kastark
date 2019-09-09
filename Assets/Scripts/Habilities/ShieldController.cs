using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class ShieldController : MonoBehaviour
{
    void Update() {
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.y > 100) {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(mRay, out hit)) {
                var target = hit.transform.gameObject;
                if (target.CompareTag("Team 1")) {
                    EventController.TriggerEvent(new ConfirmSelectedHabilityEvent());
                }
            }
        }
    }
}
