using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class CreatureOnHoverHandler : MonoBehaviour
{
    [SerializeField] CursorSkin _cursorSkin = null;

    Team _team;

    void Start()
    {
        _team = GetComponent<CreatureController>().creature.team;
    }

    void OnMouseEnter() {
        if (GameState.actingTeam != GameState.PlayerTeam) return;

        var myTeamsTurn = GameState.actingTeam == _team;
        var hability    = GameState.selectedHability;

        if (hability == null) return;

        var hoverCursor = CursorTexture.None;

        switch (hability.DamageType) {
            case DamageType.Physical:
            case DamageType.Magical: {
                hoverCursor = myTeamsTurn ? CursorTexture.Forbidden : CursorTexture.Aggressive;
            } break;
            case DamageType.Shield:
            case DamageType.Heal: {
                hoverCursor = myTeamsTurn ? CursorTexture.Friendly : CursorTexture.Forbidden;
            } break;
        }

        if (hoverCursor != CursorTexture.None) {
            _cursorSkin.ChangeCursorTexture(hoverCursor);
        }
    }

    void OnMouseExit() {
        _cursorSkin.ChangeCursorTexture(CursorTexture.None);
    }
}
