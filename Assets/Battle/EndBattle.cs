using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum BattleId
{
    SecondBattle,
    FirstBattle
};

public class EndBattle : UpdateAsStream
{
    public StateStream<bool> battleEnded =
        new StateStream<bool>(false);

    public Creature looser = null;

    public BattleId battle = BattleId.SecondBattle;

    protected void OnEnd()
    {
        if (battle == BattleId.SecondBattle)
        {
            if (!looser.gameObject.CompareTag("Player"))
                Globals.progress =
                    GameProgress.DefeatedArthur;

            Globals.Save(Checkpoint.SecondFight);
        }
        else
        {
            if (!looser.gameObject.CompareTag("Player"))
                Globals.progress =
                    GameProgress.DefeatedOthok;

            Globals.Save(Checkpoint.FirstFight);
        }
    }

    void Awake()
    {
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
                    if (battleEnded.Value == false)
                    {
                        battleEnded.Value = true;
                        looser = creature;

                        OnEnd();
                    }
                });
        }
    }
}
