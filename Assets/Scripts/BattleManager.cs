using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class BattleManager : MonoBehaviour
{
    [SerializeField] Party _playerParty = null;
    [SerializeField] Party _enemyParty = null;

    Party _currentParty;

    void Start() {
        _currentParty = _playerParty;
    }

    void OnEnable() {
        EventController.AddListener<PartyTurnEndEvent>(OnPartyTurnEnd);
    }

    void OnDisable() {
        EventController.RemoveListener<PartyTurnEndEvent>(OnPartyTurnEnd);
    }

    void OnPartyTurnEnd(PartyTurnEndEvent e) {
        _currentParty = _currentParty == _playerParty ? _enemyParty : _playerParty;
        _currentParty.BeginTurn();
    }
}
