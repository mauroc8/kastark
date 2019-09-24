using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static readonly Team PlayerTeam = Team.Left;

    public static List<CreatureController> creaturesInBattle = null;
    public static Team                     actingTeam = Team.None;
    public static CreatureController       actingCreature = null;
    public static Hability                 selectedHability = null;
    public static Consumable               selectedConsumable = null;

    public static bool IsFromActingTeam(GameObject go)
    {
        return go.CompareTag(actingTeam == Team.Left ? "LeftTeam" : "RightTeam");
    }

    public static bool IsFromActingTeam(CreatureController creature)
    {
        return IsFromActingTeam(creature.gameObject);
    }

    public static bool IsFromEnemyTeam(GameObject go)
    {
        return go.CompareTag(actingTeam == Team.Left ? "RightTeam" : "LeftTeam");
    }

    public static bool IsFromEnemyTeam(CreatureController creature)
    {
        return IsFromEnemyTeam(creature.gameObject);
    }
    
    public static Team GetTeam(GameObject go)
    {
        return go.CompareTag("LeftTeam") ? Team.Left : Team.Right;
    }

    public static Team GetTeamOf(CreatureController creature)
    {
        return GetTeam(creature.gameObject);
    }

    public static bool IsPlayersTurn()
    {
        return PlayerTeam == actingTeam;
    }
}
