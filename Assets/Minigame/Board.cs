using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Board : UpdateAsStream
{
    public BlockDictionary dictionary =
        null;

    int height;
    int width;

    public StateStream<Optional<Movement>> movement =
        new StateStream<Optional<Movement>>(Optional.None<Movement>());

    public StateStream<bool> isInRightOrder =
        new StateStream<bool>(true);

    Block[] blocks;

    public void Init(MinigameTag minigameTag)
    {
        switch (minigameTag)
        {
            case MinigameTag.Garbage:
                blocks = Globals.garbageBlocks;
                width = 3;
                height = 3;
                break;

            case MinigameTag.PuzzleA:
                blocks = Globals.puzzleABlocks;
                width = 2;
                height = 3;
                break;

            case MinigameTag.PuzzleB2:
                blocks = Globals.puzzleB2Blocks;
                width = 3;
                height = 3;
                break;

            case MinigameTag.PuzzleC:
                blocks = Globals.puzzleCBlocks;
                width = 3;
                height = 3;
                break;
        }

        if (blocks == null)
        {
            Debug.LogError($"Null board in {minigameTag}");
            return;
        }

        if (dictionary == null)
        {
            Debug.LogError($"Null board-dictionary in {minigameTag}");
            return;
        }

        var boardData =
            Query
                .From(this, "board-data")
                .Get<Transform>();

        var gameObjects =
            new GameObject[blocks.Length];

        for (int i = 0; i < blocks.Length; i++)
        {
            var block =
                blocks[i];

            var prefab =
                dictionary.GetPrefab(block.tag);

            if (prefab == null)
            {
                Debug.LogError($"Block with null prefab: {block.tag}");
                continue;
            }

            var instance =
                GameObject.Instantiate(prefab, boardData);

            instance.name =
                $"{block.tag} {block.x} {block.y}";

            instance.transform.localPosition =
                new Vector3((float)block.x, (float)block.y, 0.0f);

            gameObjects[i] =
                instance;
        }

        // --- Slots ---

        var slots =
            Query
                .From(this, "slots")
                .Get<Transform>();

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var slot =
                    new GameObject($"slot {x} {y}");

                slot
                    .AddComponent<BoxCollider>();

                slot.layer =
                    10;

                slot.transform
                    .SetParent(slots);

                slot.transform.localPosition =
                    new Vector3(
                        (float)x,
                        (float)y,
                        0.0f
                    );

                slot.transform.localRotation =
                    Quaternion.identity;

                slot.transform.localScale =
                    new Vector3(1, 1, 1);
            }


        // --- Movement ---

        movement
            .AndThen(movement =>
                movement
                    .CaseOf(update.Always, Stream.None<Movement>)
            )
            .Get(currentMovement =>
            {
                var block =
                    currentMovement.block;

                var target =
                    currentMovement.target;

                var blockIdx =
                    Array.IndexOf(blocks, block);

                if (blockIdx == -1)
                {
                    Debug.LogError($"Movement with invalid block {block}");
                    return;
                }
                var gameObject =
                    gameObjects[blockIdx];

                //

                var t =
                    (Time.time - currentMovement.startTime) / Movement.Duration;

                gameObject.transform.localPosition =
                    Vector3.Lerp(
                        new Vector3(
                            block.x,
                            block.y,
                            0.0f
                        ),
                        new Vector3(
                            target.Item1,
                            target.Item2,
                            0.0f
                        ),
                        Mathf.Pow(t, 0.5f)
                    );

                if (t >= 1)
                {
                    block.x =
                        target.Item1;

                    block.y =
                        target.Item2;

                    blocks[blockIdx] =
                        block;

                    movement.Value =
                        Optional.None<Movement>();
                }
            });

        movement
            .Initialized
            .Filter(value => value.CaseOf(_ => false, () => true))
            .Get(_ =>
            {
                var rightOrder =
                    IsInRightOrder(minigameTag, blocks);

                if (rightOrder != isInRightOrder.Value)
                    isInRightOrder.Value =
                        rightOrder;
            });

        // Hide mock board

        var mock =
            Query
                .From(this, "mock")
                .Get();

        mock.SetActive(false);
    }

    public bool TryPushMovement(Movement newMovement)
    {
        var isAnimating =
            movement.Value.ToBool();

        if (!isAnimating)
        {
            movement.Value =
                Optional.Some(newMovement);

            return true;
        }

        return false;
    }

    public Optional<Block> TryGetBlockAt((int, int) coords)
    {
        var (x, y) =
            coords;

        foreach (var block in blocks)
        {
            if (block.CollidesAt(x, y))
            {
                return Optional.Some(block);
            }
        }

        return Optional.None<Block>();
    }

    public Optional<(int, int)> TryGetValidDelta(Block block, (int, int) target)
    {
        var (x, y) =
            target;

        if (block.CollidesAt(x, y))
            // Target is inside the block
            return Optional.None<(int, int)>();

        foreach (var (dx, dy) in new (int, int)[4] { (1, 0), (-1, 0), (0, 1), (0, -1) })
        {
            var movedBlocks =
                new Block[]
                {
                    block
                        .WithX(block.x + 1 * dx)
                        .WithY(block.y + 1 * dy),
                    block
                        .WithX(block.x + 2 * dx)
                        .WithY(block.y + 2 * dy)
                };

            for (int i = 0; i < movedBlocks.Length; i++)
            {
                if (!movedBlocks[i].CollidesAt(x, y))
                    continue;

                var hasConflict =
                    false;

                foreach (var boardBlock in blocks)
                {
                    if (boardBlock.x == block.x &&
                        boardBlock.y == block.y)
                        continue;

                    for (int j = 0; j <= i && !hasConflict; j++)
                    {
                        if (boardBlock.CollidesWith(movedBlocks[j]))
                            hasConflict = true;
                    }

                    if (hasConflict)
                        break;
                }

                if (!hasConflict)
                    return Optional.Some(((1 + i) * dx, (1 + i) * dy));

                break;
            }
        }

        return Optional.None<(int, int)>();
    }


    bool IsInRightOrder(MinigameTag minigameTag, Block[] blocks)
    {
        if (minigameTag == MinigameTag.Garbage)
            return false;

        if (minigameTag == MinigameTag.PuzzleA)
        {
            foreach (var block in blocks)
            {
                var index =
                    ((PuzzleABlock)block.tag).index;

                if (
                    (index == 0 && (block.x != 0 || block.y != 0)) ||
                    (index == 1 && (block.x != 0 || block.y != 1)) ||
                    (index == 2 && (block.x != 1 || block.y != 1)) ||
                    (index == 3 && (block.x != 0 || block.y != 2)) ||
                    (index == 4 && (block.x != 1 || block.y != 2))
                    )
                    return false;
            }
            return true;
        }

        if (minigameTag == MinigameTag.PuzzleB2)
        {
            foreach (var block in blocks)
            {
                var index =
                    ((PuzzleB2Block)block.tag).index;

                if (
                    (index == 0 && (block.x != 0 || block.y != 0)) ||
                    (index == 1 && (block.x != 2 || block.y != 0)) ||
                    (index == 2 && (block.x != 0 || block.y != 1)) ||
                    (index == 3 && (block.x != 1 || block.y != 1)) ||
                    (index == 4 && (block.x != 2 || block.y != 1)) ||
                    (index == 5 && (block.x != 1 || block.y != 2))
                    )
                    return false;
            }
            return true;
        }

        if (minigameTag == MinigameTag.PuzzleC)
        {
            foreach (var block in blocks)
            {
                var index =
                    ((PuzzleCBlock)block.tag).index;

                if (
                    (index == 0 && (block.x != 1 || block.y != 0)) ||
                    (index == 1 && (block.x != 2 || block.y != 0)) ||
                    (index == 2 && (block.x != 0 || block.y != 1)) ||
                    (index == 3 && (block.x != 1 || block.y != 1)) ||
                    (index == 4 && (block.x != 2 || block.y != 1)) ||
                    (index == 5 && (block.x != 0 || block.y != 2)) ||
                    (index == 6 && (block.x != 1 || block.y != 2)) ||
                    (index == 7 && (block.x != 2 || block.y != 2))
                    )
                    return false;
            }
            return true;
        }

        return false;
    }
}
