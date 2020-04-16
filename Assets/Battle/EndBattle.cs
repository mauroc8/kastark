using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class BattleWinStatus
{
    public static bool battleEnded;
    public static Creature looser;
}

public class EndBattle : UpdateAsStream
{
    void Awake()
    {
        BattleWinStatus.battleEnded = false;

        var creatures = GetComponentsInChildren<Creature>();

        foreach (var creature in creatures)
        {
            creature.State
                .Filter(state =>
                    state.lifePoints
                        .Where(lifePoint => lifePoint != LifePointState.Dead)
                        .Count()
                        == 0
                )
                .Lazy()
                .Get(_ =>
                {
                    if (BattleWinStatus.battleEnded == false)
                    {
                        BattleWinStatus.battleEnded = true;
                        BattleWinStatus.looser = creature;
                    }
                });
        }
    }
}
