using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;
using System.Linq;

public abstract class Movement
{
    public void ApplyTo(Board.Block[] blocks, GameObject[] gameObjects, int width, int height)
    {
        if (!HasEnded)
        {
            _ApplyTo(blocks, gameObjects, width, height);
        }
    }

    protected abstract void _ApplyTo(Board.Block[] blocks, GameObject[] gameObjects, int width, int height);

    public abstract bool HasEnded { get; }
}

public class SimpleMovement : Movement
{
    public Vector3 anchor;
    public Vector3 focus;
    public bool userGenerated;
    public float startTime;

    float movementDuration => 0.3f;

    bool hasEnded = false;

    public override bool HasEnded => hasEnded;

    protected override void _ApplyTo(Board.Block[] blocks, GameObject[] gameObjects, int width, int height)
    {
        var t = (Time.time - startTime) / movementDuration;

        var anchorIdx = (int)anchor.x + (int)anchor.y * width;
        var focusIdx = (int)focus.x + (int)focus.y * width;

        gameObjects[anchorIdx].transform.localPosition =
            Vector3.Lerp(
                anchor,
                focus,
                t
            )
            // Prevent z-fighting
            .WithZ(t < 1.0f ? -0.05f : 0.0f);

        gameObjects[focusIdx].transform.localPosition =
            Vector3.Lerp(
                focus,
                anchor,
                t
            );

        if (t >= 1 && !hasEnded)
        {
            var tmp0 = blocks[anchorIdx];
            blocks[anchorIdx] = blocks[focusIdx];
            blocks[focusIdx] = tmp0;

            var tmp1 = gameObjects[anchorIdx];
            gameObjects[anchorIdx] = gameObjects[focusIdx];
            gameObjects[focusIdx] = tmp1;

            hasEnded = true;
        }
    }
}

public class ClearRowMovement : Movement
{
    public int row;
    public float startTime;

    bool hasDestroyedRow = false;
    bool hasEnded = false;

    public override bool HasEnded => hasEnded;

    protected override void _ApplyTo(Board.Block[] blocks, GameObject[] gameObjects, int width, int height)
    {
        // The first half of the animation we will destroy the row,
        // The second half, we will apply "gravity" to the blocks on top.

        var destroyDuration = 0.3f;
        var moveDownDuration = 0.3f;

        var t0 = (Time.time - startTime) / destroyDuration;
        var t1 = (Time.time - (startTime + destroyDuration)) / moveDownDuration;
        var firstAnimation = t0 <= 1;

        if (firstAnimation)
        {
            // Animate row destroying
            for (int x = 0; x < width; x++)
            {
                var gameObject = gameObjects[x + row * width];

                gameObject.transform.localScale =
                    new Vector3(1 - t0, 1 - t0, 1 - t0);
            }
        }
        else
        {
            // Destroy row & animate other rows falling down
            if (!hasDestroyedRow)
            {
                for (int x = 0; x < width; x++)
                {
                    var gameObject = gameObjects[x + row * width];
                    GameObject.Destroy(gameObject);

                    gameObjects[x + row * width] = null;
                    blocks[x + row * width] = Board.Block.Empty;
                }

                hasDestroyedRow = true;
            }

            // Move all blocks from rows beneath `row`.
            for (int y = row + 1; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var fromIdx = x + y * width;
                    var toIdx = x + (y - 1) * width;

                    var gameObject = gameObjects[fromIdx];

                    if (gameObject != null)
                    {
                        gameObject.transform.localPosition =
                            new Vector3(x, Mathf.Lerp(y, y - 1, t1), 0.0f);
                    }

                    if (t1 >= 1 && !hasEnded)
                    {
                        var tmp0 = blocks[fromIdx];
                        blocks[fromIdx] = blocks[toIdx];
                        blocks[toIdx] = tmp0;

                        var tmp1 = gameObjects[fromIdx];
                        gameObjects[fromIdx] = gameObjects[toIdx];
                        gameObjects[toIdx] = tmp1;
                    }
                }
            }

            if (t1 >= 1 && !hasEnded)
            {
                hasEnded = true;
            }
        }

    }
}

public class ClearColumnMovement : Movement
{
    public int column;
    public float startTime;


    float movementDuration => 0.3f;

    bool hasEnded = false;

    public override bool HasEnded => hasEnded;

    protected override void _ApplyTo(Board.Block[] blocks, GameObject[] gameObjects, int width, int height)
    {
        var t = (Time.time - startTime) / movementDuration;

        for (int y = 0; y < height; y++)
        {
            var gameObject = gameObjects[column + y * width];

            if (gameObject == null)
            {
                Debug.Log($"Trying to remove block {column}, {y}. In board, this is {blocks[column + y * width]}. " +
                    $"The animation frame is {t}, {HasEnded}.");
            }

            var k =
                Mathf.Lerp(1, 0, t);

            gameObject.transform.localScale =
                new Vector3(k, k, k);

            if (!hasEnded && t >= 1)
            {
                GameObject.Destroy(gameObject);

                gameObjects[column + y * width] = null;
                blocks[column + y * width] = Board.Block.Empty;
            }
        }

        if (!hasEnded && t >= 1)
        {
            hasEnded = true;
        }
    }
}

public class MultipleMovements : Movement
{
    public List<Movement> movements = new List<Movement>();

    public override bool HasEnded =>
        movements
            .All(movement => movement.HasEnded);

    protected override void _ApplyTo(Board.Block[] blocks, GameObject[] gameObjects, int width, int height)
    {
        foreach (var movement in movements)
        {
            movement.ApplyTo(blocks, gameObjects, width, height);
        }
    }
}
