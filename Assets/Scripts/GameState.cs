using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameState _instance;
    
    void Awake() {
        Debug.Assert(_instance == null);
        _instance = this;
    }

    public static GameState Instance {
        get {
            Debug.Assert(_instance != null);
            return _instance;
        }
    }

    public Creature[] battleParticipants;
    public Creature   actingUnit;

    public string     PlayerTeamTag = "LeftTeam";
    public string     EnemyTeamTag  = "RightTeam";
}
