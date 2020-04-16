using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Battle : MonoBehaviour
{
    public void CreatureBeganAttack(TeamId team)
    {
        foreach (var creature in
            GetComponentsInChildren<Creature>()
                .Where(creature => creature.team != team)
            )
            creature.AnimateLifePoints();
    }

    public void CreatureCeasedAttack(TeamId team)
    {
        foreach (var creature in
            GetComponentsInChildren<Creature>()
                .Where(creature => creature.team != team)
            )
            creature.PauseLifePoints();
    }
}
