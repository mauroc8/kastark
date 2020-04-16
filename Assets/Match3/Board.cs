using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

public class Board : UpdateAsStream
{
    public GameObject woodPrefab = null;
    public GameObject plasticPrefab = null;
    public GameObject organicPrefab = null;

    StreamSource<Movement> movement =
        new StreamSource<Movement>();

    bool _animating = false;
    StreamSource<bool> animating = new StreamSource<bool>();

    void Start()
    {
        animating.Push(_animating);
    }

    public enum Block
    {
        Wood,
        Plastic,
        Organic,
        Empty
    }

    void Awake()
    {
        // Create board

        Block[] blocks = new Block[]
        {
            Block.Organic, Block.Wood, Block.Wood,
            Block.Plastic, Block.Organic, Block.Plastic,
            Block.Wood, Block.Plastic, Block.Organic
        };

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
                    null;

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
                currentMovement.ApplyTo(blocks, gameObjects, width, height);

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
                            animating.Push(_animating = false);
                            break;
                    }
                }
            });
    }

    public void CommitMovement(Vector3 anchor, Vector3 focus)
    {
        if (!_animating)
        {
            // Verify movement:
            if (anchor.z != 0
                || focus.z != 0
                || Vector3.Distance(anchor, focus) > 1.0f)
                return;

            // Push movement
            movement.Push(new SimpleMovement
            {
                anchor = anchor,
                focus = focus,
                startTime = Time.time,
                userGenerated = true
            });

            // Start animation
            animating.Push(_animating = true);
        }
    }

    private Optional<Movement> GetNextMovement(
        Movement lastMovement,
        Block[] blocks,
        int width,
        int height)
    {
        // If the last movement created columns,
        // now we will clear that columns.
        var clearColumns = new List<Movement>();

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
                clearColumns.Add(new ClearColumnMovement
                {
                    column = x,
                    startTime = Time.time
                });
            }
        }

        if (clearColumns.Count > 0)
        {
            return Optional.Some<Movement>(new MultipleMovements
            {
                movements = clearColumns
            });
        }

        // If the last movement created rows, we clear them.
        var clearRows = new List<Movement>();

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
                clearRows.Add(new ClearRowMovement
                {
                    row = y,
                    startTime = Time.time
                });
            }
        }

        if (clearRows.Count > 0)
        {
            return Optional.Some<Movement>(new MultipleMovements
            {
                movements = clearRows
            });
        }

        // No row or column was affected ...

        // If the last movement was an invalid user-generated movement, we rollback.

        var simpleMovement = lastMovement as SimpleMovement;

        if (simpleMovement != null && simpleMovement.userGenerated)
        {
            return Optional.Some<Movement>(new SimpleMovement
            {
                anchor = simpleMovement.focus,
                focus = simpleMovement.anchor,
                startTime = Time.time,
                userGenerated = false
            });
        }

        // Nothing to do here. This means the last movement
        // was a rollback of a previous invalid movement.

        return Optional.None<Movement>();
    }
}

