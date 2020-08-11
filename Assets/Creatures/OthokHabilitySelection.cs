using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class OthokHabilitySelection : MonoBehaviour
{
    void Awake()
    {
        var battle =
            GetComponentInParent<Battle>();

        var creature =
            GetComponentInParent<Creature>();

        var isChoosingHability =
            battle
                .turn
                .Map(optionalTurn =>
                    optionalTurn.CaseOf(
                        turn =>
                            battle.ActingCreature(turn) == creature
                                && turn.action == TurnAction.SelectAbility,
                        () => false
                    )
                )
                .Lazy();

        var ability =
            creature
                .GetComponentInChildren<Ability>(true);

        isChoosingHability
            .Filter(a => a)
            .Get(_ =>
            {
                StartCoroutine(SelectAbilityCoroutine(ability));
            });

    }

    IEnumerator SelectAbilityCoroutine(Ability ability)
    {
        yield return new WaitForSeconds(1.0f);

        var battle =
            GetComponentInParent<Battle>();

        battle.CreatureSelectsAbility(ability);
    }
}
