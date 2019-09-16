using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class HabilityCursorOnHover : MonoBehaviour
{
    TeamSide _teamSide;

    void Start() {
        _teamSide = gameObject.CompareTag("LeftTeam") ? TeamSide.Left : TeamSide.Right;
    }

    void OnEnable() {
        EventController.AddListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.AddListener<HabilityCastStartEvent>(OnHabilityCastStart);
    }
    void OnDisable() {
        EventController.RemoveListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.RemoveListener<HabilityCastStartEvent>(OnHabilityCastStart);
    }
    void OnHabilitySelect(HabilitySelectEvent e) {
        bool playerTeam = GameState.actingTeam == _teamSide;

        switch (e.habilityId) {
            case HabilityId.Attack:
            case HabilityId.Magic: {
                _hoverCursor = playerTeam ? CursorTexture.Forbidden : CursorTexture.Aggressive;
            } break;
            case HabilityId.Shield:
            case HabilityId.Heal: {
                _hoverCursor = playerTeam ? CursorTexture.Friendly : CursorTexture.Forbidden;
            } break;
        }
    }

    void OnHabilityCastStart(HabilityCastStartEvent e) {
        _hoverCursor = CursorTexture.None;
    }

    CursorTexture _hoverCursor = CursorTexture.None;

    void OnMouseEnter() {
        if (_hoverCursor != CursorTexture.None) {
            CursorController.ChangeCursorTexture(_hoverCursor);
        }
    }
    void OnMouseExit() {
        if (_hoverCursor != CursorTexture.None) {
            CursorController.ChangeCursorTexture(CursorTexture.None);
        }
    }
}
