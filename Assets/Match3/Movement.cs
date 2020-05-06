using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;
using System.Linq;

public abstract class Movement
{
    public void ApplyTo(Block[] blocks, GameObject[] gameObjects, int width, int height, Board board)
    {
        if (!HasEnded)
        {
            _ApplyTo(blocks, gameObjects, width, height, board);
        }
    }

    protected abstract void _ApplyTo(Block[] blocks, GameObject[] gameObjects, int width, int height, Board board);

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

    protected override void _ApplyTo(Block[] blocks, GameObject[] gameObjects, int width, int height, Board board)
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

    int blockIdx = 0;

    public override bool HasEnded => blockIdx >= 3;

    protected override void _ApplyTo(Block[] blocks, GameObject[] gameObjects, int width, int height, Board board)
    {
        var duration = 0.6f;

        var t = (Time.time - startTime) / duration;

        var x =
            blockIdx;

        blockIdx =
            (int)(t * 3);

        var gameObject = gameObjects[x + row * width];

        gameObject.transform.localScale =
            new Vector3(1 - t, 1 - t, 1 - t);

        if (x < blockIdx)
        {
            board.Earn1Resource(blocks[x + row * width]);

            GameObject.Destroy(gameObject);

            var newGameObject =
                GameObject.Instantiate(
                    board.emptyPrefab,
                    board.transform);

            newGameObject.transform.localPosition =
                new Vector3(x, row, 0.0f);

            gameObjects[x + row * width] = newGameObject;

            blocks[x + row * width] = Block.Empty;
        }
    }
}

public class ClearColumnMovement : Movement
{
    public int column;
    public float startTime;


    float movementDuration => 0.6f;

    int blockIdx = 0;

    public override bool HasEnded => blockIdx >= 3;

    protected override void _ApplyTo(Block[] blocks, GameObject[] gameObjects, int width, int height, Board board)
    {
        var t = (Time.time - startTime) / movementDuration;

        var y =
            blockIdx;

        blockIdx =
            (int)(t * 3);

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

        if (y < blockIdx)
        {
            board.Earn1Resource(blocks[column + y * width]);

            GameObject.Destroy(gameObject);

            var newGameObject =
                GameObject.Instantiate(
                    board.emptyPrefab,
                    board.transform);

            newGameObject.transform.localPosition =
                new Vector3(column, y, 0.0f);

            gameObjects[column + y * width] = newGameObject;

            blocks[column + y * width] = Block.Empty;
        }
    }
}

public class MultipleMovements : Movement
{
    public List<Movement> movements = new List<Movement>();

    public override bool HasEnded =>
        movements
            .All(movement => movement.HasEnded);

    protected override
    void _ApplyTo(Block[] blocks, GameObject[] gameObjects, int width, int height, Board board)
    {
        foreach (var movement in movements)
        {
            movement.ApplyTo(blocks, gameObjects, width, height, board);
        }
    }
}
