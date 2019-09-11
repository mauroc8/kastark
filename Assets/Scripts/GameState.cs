using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static Creature[] unitsInBattle;
    public static Creature   currentUnit;

    public static string     PlayerTeamTag = "LeftTeam";
    public static string     EnemyTeamTag  = "RightTeam";
}
