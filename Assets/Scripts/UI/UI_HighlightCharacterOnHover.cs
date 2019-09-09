using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class UI_HighlightCharacterOnHover : MonoBehaviour
{
    [SerializeField] TeamId _teamId = TeamId.PlayerTeam;

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
        EventController.AddListener<SelectedHabilityEvent>(OnSelectHability);
        EventController.AddListener<ConfirmSelectedHabilityEvent>(OnConfirmHability);
    }
    void OnDisable() {
        EventController.RemoveListener<SelectedHabilityEvent>(OnSelectHability);
        EventController.RemoveListener<ConfirmSelectedHabilityEvent>(OnConfirmHability);
    }
    void OnSelectHability(SelectedHabilityEvent e) {
        Color outlineColor;
        var habilityId = e.habilityDescription.habilityId;

        if (habilityId == HabilityId.Shield) {
            outlineColor = _teamId == TeamId.PlayerTeam ? _goodColor : _neutralColor;
        } else {
            outlineColor = _teamId == TeamId.PlayerTeam ? _neutralColor : _badColor;
        }

        _material.SetColor("_OutlineColor", outlineColor);
    }

    void OnConfirmHability(ConfirmSelectedHabilityEvent e) {
        _material.SetColor("_OutlineColor", _neutralColor);
    }
}
