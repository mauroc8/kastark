using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class CastHabilityUIController : MonoBehaviour
{
    [SerializeField]
    SelectHabilityUIController _selectHabilityUIController = null;

    [SerializeField]
    CanvasGroup _habilityDescriptionPanel = null;

    bool _selectedHabilityIsLocked = false;

    void Update() {
        if (_selectedHabilityIsLocked) return;

        Vector2 mousePos = (Vector2) Input.mousePosition;

        float alpha =
            mousePos.y < 104 ? 1 :
            mousePos.y > 134 ? 0.9f :
            1 - (1 - 0.9f) * (mousePos.y - 104) / (134 - 104);

        _habilityDescriptionPanel.alpha = alpha;

        if (Input.GetMouseButtonDown(0) && alpha <= 0.9f) {
            var selectedHability = _selectHabilityUIController.GetSelectedHabilityInfo();
            EventController.TriggerEvent(new HabilityCastEvent{ habilityInfo = selectedHability });

            _habilityDescriptionPanel.alpha = 0.9f;
            _selectHabilityUIController.LockSelectedHability();
            _selectedHabilityIsLocked = true;
        }
    }
}
