using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionController : Ability
{
    public override bool IsAgressive =>
        false;

    int heal = 5;

    void Awake()
    {
        var creature =
            GetComponentInParent<Creature>();

        var battle =
            GetComponentInParent<Battle>();

        var isCasting =
            battle
                .turn
                .Map(optionalTurn =>
                    optionalTurn.CaseOf(
                        turn =>
                            battle.ActingCreature(turn) == creature
                                && turn.action == TurnAction.CastAbility
                                && turn.selectedAbility == this,
                        () => false
                    )
                )
                .Lazy();

        var castStart =
            isCasting
                .Filter(a => a);


        castStart
            .Get(_ =>
            {
                StartCoroutine(DrinkPotionCoroutine(creature, battle));
            });
    }

    IEnumerator DrinkPotionCoroutine(Creature creature, Battle battle)
    {
        Globals.PotionAmount--;

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < heal; i++)
        {
            yield return new WaitForSeconds(0.35f);

            creature.HealthPotionWasDrank(1);
        }

        battle.CreatureEndsTurn();
    }

}
