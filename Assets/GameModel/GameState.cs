using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static readonly Team PlayerTeam = Team.Left;

    public static Team   actingTeam;
    public static CreatureController   actingCreature;
    public static Hability   selectedHability;

    public static bool IsFromActingTeam(GameObject go)
    {
        return go.CompareTag(actingTeam == Team.Left ? "LeftTeam" : "RightTeam");
    }

    public static bool IsFromActingTeam(CreatureController creature)
    {
        return IsFromActingTeam(creature.gameObject);
    }
    
    public static Team GetTeam(GameObject go)
    {
        return go.CompareTag("LeftTeam") ? Team.Left : Team.Right;
    }

    public static Team GetTeam(CreatureController creature)
    {
        return GetTeam(creature.gameObject);
    }

    public static bool IsPlayersTurn()
    {
        return PlayerTeam == actingTeam;
    }
}
