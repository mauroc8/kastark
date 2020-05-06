using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CheckpointsController : MonoBehaviour
{
    void Awake()
    {
        var player =
            Query
            .From(transform.root, "player")
            .Get<NavMeshAgent>();

        var checkpoint =
            Query
                .From(
                    this,
                    Globals.checkpoint == Checkpoint.Initial ? "initial" :
                    Globals.checkpoint == Checkpoint.FirstMatch3 ? "first-match3" :
                    Globals.checkpoint == Checkpoint.SecondMatch3 ? "second-match3" :
                    Globals.checkpoint == Checkpoint.Dwarf ? "dwarf" :
                    Globals.checkpoint == Checkpoint.SecondFight ? "second-fight" :
                    Globals.checkpoint == Checkpoint.FirstFight ? "first-fight" :
                    $"missing checkpoint {Globals.checkpoint}"
                )
                .Get<Transform>();

        player.Warp(
            checkpoint.position
        );
    }
}
