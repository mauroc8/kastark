using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static readonly TeamSide PlayerTeam = TeamSide.Left;

    public static TeamSide   actingTeam;
    public static Creature   actingCreature;
    public static Hability   selectedHability;

    public static bool IsFromActingTeam(GameObject go) {
        return
            go.CompareTag(actingTeam == TeamSide.Left ? "TeamLeft" : "TeamRight");
    }
}
