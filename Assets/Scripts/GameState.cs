using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameState _instance;
    
    public static GameState Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<GameState>() as GameState;
                Debug.Assert(_instance);
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    public static Creature[] battleParticipants;
    public static TeamSide   actingTeam;
    public static Creature   actingCreature;

    public static string     PlayerTeamTag = "LeftTeam";
    public static string     EnemyTeamTag  = "RightTeam";
}
