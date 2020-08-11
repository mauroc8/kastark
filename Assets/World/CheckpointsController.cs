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

        var initial =
            Query.From(this, "initial").Get<Transform>();

        var puzzleA =
            Query.From(this, "puzzle-a").Get<Transform>();

        var puzzleB2 =
            Query.From(this, "puzzle-b2").Get<Transform>();

        var puzzleC =
            Query.From(this, "puzzle-c").Get<Transform>();

        var garbage =
            Query.From(this, "garbage").Get<Transform>();

        var enemy0 =
            Query.From(this, "enemy-0").Get<Transform>();

        var enemy1 =
            Query.From(this, "enemy-1").Get<Transform>();

        var enemy2 =
            Query.From(this, "enemy-2").Get<Transform>();

        var enemy3 =
            Query.From(this, "enemy-3").Get<Transform>();

        var arthur =
            Query.From(this, "arthur").Get<Transform>();

        var dhende =
            Query.From(this, "dhende").Get<Transform>();

        var checkpoint =
            Globals.checkpoint == Checkpoint.Initial ? initial :
            Globals.checkpoint == Checkpoint.PuzzleA ? puzzleA :
            Globals.checkpoint == Checkpoint.PuzzleB2 ? puzzleB2 :
            Globals.checkpoint == Checkpoint.PuzzleC ? puzzleC :
            Globals.checkpoint == Checkpoint.Garbage ? garbage :
            Globals.checkpoint == Checkpoint.Dhende ? dhende :
            Globals.checkpoint == Checkpoint.Enemy0 ? enemy0 :
            Globals.checkpoint == Checkpoint.Enemy1 ? enemy1 :
            Globals.checkpoint == Checkpoint.Enemy2 ? enemy2 :
            Globals.checkpoint == Checkpoint.Enemy3 ? enemy3 :
            Globals.checkpoint == Checkpoint.Arthur ? arthur :
            Globals.checkpoint == Checkpoint.Garbage ? garbage :
            initial;

        player.Warp(
            checkpoint.position
        );
    }
}
