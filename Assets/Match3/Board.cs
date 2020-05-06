using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

public enum BoardTag
{
    FirstBoard,
    SecondBoard
};

public enum Block
{
    Wood,
    Plastic,
    Organic,
    Empty
}

public class Board : UpdateAsStream
{
    public BoardTag boardTag;

    public GameObject woodPrefab = null;
    public GameObject plasticPrefab = null;
    public GameObject organicPrefab = null;
    public GameObject emptyPrefab = null;

    EventStream<Movement> movement =
        new EventStream<Movement>();

    StateStream<bool> animating = new StateStream<bool>(false);

    void Start()
    {
        animating.Value =
            animating.Value;
    }

    void Awake()
    {
        // Create board

        Block[] blocks;

        if (!Globals.boards.TryGetValue(boardTag, out blocks))
        {
            Debug.LogError($"Error loading board");
            return;
        }

        int width = 3;
        int height = 3;

        var gameObjects = new GameObject[width * height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var block = blocks[x + y * width];

                var prefab =
                    block == Block.Wood ? woodPrefab :
                    block == Block.Organic ? organicPrefab :
                    block == Block.Plastic ? plasticPrefab :
                    emptyPrefab;

                if (prefab != null)
                {
                    var instance =
                        GameObject.Instantiate(prefab, transform);

                    instance.transform.localPosition =
                        new Vector3(x, y, 0);

                    instance.name = $"block-{x}-{y}";

                    gameObjects[x + y * width] = instance;
                }
            }

        // Animate user movement

        Stream.Combine(animating, movement)
            .AndThen((animating, movement) =>
                animating
                    ? update.Always(movement)
                    : Stream.None<Movement>()
            )
            .Get(currentMovement =>
            {
                currentMovement.ApplyTo(blocks, gameObjects, width, height, this);

                if (currentMovement.HasEnded)
                {
                    var nextMovement =
                        GetNextMovement(currentMovement, blocks, width, height);

                    switch (nextMovement)
                    {
                        case Some<Movement> m:
                            movement.Push(m.Value);
                            break;
                        case None<Movement> _:
                            animating.Value = false;
                            break;
                    }
                }
            });
    }

    public bool TryCommitMovement(Vector3 anchor, Vector3 focus)
    {
        if (animating.Value)
            return false;

        // Verify movement:
        if (anchor.z != 0
            || focus.z != 0
            || Vector3.Distance(anchor, focus) > 1.0f)
            return false;

        if (anchor.x == focus.x && anchor.y == focus.y)
            return false;

        // Push movement
        movement.Push(new SimpleMovement
        {
            anchor = anchor,
            focus = focus,
            startTime = Time.time,
            userGenerated = true
        });

        // Start animation
        animating.Value = true;

        return true;
    }

    private Optional<Movement> GetNextMovement(
        Movement lastMovement,
        Block[] blocks,
        int width,
        int height)
    {
        for (int x = 0; x < width; x++)
        {
            var block = blocks[x];

            if (block == Block.Empty)
                continue;

            var canClearColumn = true;

            for (int y = 0; y < height; y++)
            {
                if (blocks[x + y * width] != block)
                {
                    canClearColumn = false;
                    break;
                }
            }

            if (canClearColumn)
            {
                return
                    Optional.Just(new ClearColumnMovement
                    {
                        column = x,
                        startTime = Time.time
                    });
            }
        }

        // If the last movement created rows, we clear them.

        for (int y = height - 1; y >= 0; y--)
        {
            // We loop in reversed sense because
            // order matters -- remember we're swaping things around.

            var block = blocks[y * width];

            if (block == Block.Empty)
                continue;

            var canClearRow = true;

            for (int x = 0; x < width; x++)
            {
                if (blocks[x + y * width] != block)
                {
                    canClearRow = false;
                    break;
                }
            }

            if (canClearRow)
            {
                return
                    Optional.Just(new ClearRowMovement
                    {
                        row = y,
                        startTime = Time.time
                    });
            }
        }

        // If the last movement was an invalid user-generated movement, we rollback.

        // var simpleMovement = lastMovement as SimpleMovement;

        // if (simpleMovement != null && simpleMovement.userGenerated)
        // {
        //     return Optional.Some<Movement>(new SimpleMovement
        //     {
        //         anchor = simpleMovement.focus,
        //         focus = simpleMovement.anchor,
        //         startTime = Time.time,
        //         userGenerated = false
        //     });
        // }

        // Nothing to do here.

        return Optional.None<Movement>();
    }

    public EventStream<Block> earnedResources = new EventStream<Block>();
    public StateStream<bool> isCleared = new StateStream<bool>(false);

    int earnedResourcesAmount = 0;

    public void Earn1Resource(Block resource)
    {
        earnedResources.Push(resource);

        earnedResourcesAmount++;

        if (earnedResourcesAmount >= 9)
            isCleared.Push(true);
    }
}

