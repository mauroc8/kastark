using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastHabilityManager : MonoBehaviour
{
    [SerializeField]
    GameObject _cancelButton = null;

    bool _casting = false;

    void Update() {
        if (!_casting && Input.GetMouseButtonDown(0)) {
            StartCasting();
        }
    }

    void StartCasting() {
        _casting = true;

        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(mRay, out hit)){
            if (hit.transform.gameObject.CompareTag("Team 2")) {
                var enemy = hit.transform.gameObject;
                Debug.Log($"Cast hability ### on enemy {enemy.name}");
            }
        }
    }
}
