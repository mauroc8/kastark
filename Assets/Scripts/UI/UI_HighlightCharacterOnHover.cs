using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class UI_HighlightCharacterOnHover : MonoBehaviour
{
    [SerializeField] TeamSide _teamSide = TeamSide.Left;

    [SerializeField] Color _goodColor = Color.green;
    [SerializeField] Color _badColor = Color.red;
    [SerializeField] Color _neutralColor = Color.gray;

    [SerializeField] float _onHoverOutlineWidth = 0.07f;

    Material _material;

    void Start() {
        _material = GetComponent<Renderer>().material;
        _material.SetColor("_OutlineColor", _neutralColor);
    }

    void OnMouseEnter() {
        _material.SetFloat("_OutlineWidth", _onHoverOutlineWidth);
    }

    void OnMouseExit() {
        _material.SetFloat("_OutlineWidth", 0);
    }

    //public class SelectedHabilityEvent : GameEvent { public HabilityDescription habilityDescription; }
    //public class ConfirmSelectedHabilityEvent : GameEvent { }

    void OnEnable() {
        EventController.AddListener<HabilitySelectEvent>(OnSelectHability);
        EventController.AddListener<HabilityCastStartEvent>(OnConfirmHability);
    }
    void OnDisable() {
        EventController.RemoveListener<HabilitySelectEvent>(OnSelectHability);
        EventController.RemoveListener<HabilityCastStartEvent>(OnConfirmHability);
    }
    void OnSelectHability(HabilitySelectEvent e) {
        Color outlineColor;
        var habilityId = e.habilityId;

        if (habilityId == HabilityId.Shield || habilityId == HabilityId.Heal) {
            outlineColor = _teamSide == TeamSide.Left ? _goodColor : _neutralColor;
        } else {
            outlineColor = _teamSide == TeamSide.Left ? _neutralColor : _badColor;
        }

        _material.SetColor("_OutlineColor", outlineColor);
    }

    void OnConfirmHability(HabilityCastStartEvent e) {
        _material.SetColor("_OutlineColor", _neutralColor);
    }
}
