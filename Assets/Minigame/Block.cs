using UnityEngine;

public interface BlockTag { }

public struct CanBlock : BlockTag { }
public struct BottleBlock : BlockTag { }
public struct WideBottleBlock : BlockTag { }

public struct PuzzleABlock : BlockTag
{
    public int index;
}

public struct PuzzleB2Block : BlockTag
{
    public int index;
}

public struct PuzzleCBlock : BlockTag
{
    public int index;
}

public struct Block
{
    public BlockTag tag;

    public int x;
    public int y;
    public int width;
    public int height;

    public Block(BlockTag tag, int x, int y, int width, int height)
    {
        this.tag = tag;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    // --- Garbage blocks ---

    public static Block Can(int x, int y)
    {
        return new Block(new CanBlock(), x, y, 1, 1);
    }

    public static Block Bottle(int x, int y)
    {
        return new Block(new BottleBlock(), x, y, 1, 1);
    }

    public static Block WideBottle(int x, int y)
    {
        return new Block(new WideBottleBlock(), x, y, 1, 2);
    }

    // --- Puzzle blocks ---

    public static Block PuzzleA(int index, int x, int y)
    {
        return new Block(new PuzzleABlock { index = index }, x, y, 1, 1);
    }

    public static Block PuzzleB2(int index, int x, int y)
    {
        return new Block(new PuzzleB2Block { index = index }, x, y, index == 0 ? 2 : 1, 1);
    }

    public static Block PuzzleC(int index, int x, int y)
    {
        return new Block(new PuzzleCBlock { index = index }, x, y, 1, 1);
    }

    // --- Helper methods ---

    public bool CollidesAt(int x, int y)
    {
        return
            this.x <= x && x < this.x + this.width
                && this.y <= y && y < this.y + this.height;
    }

    public bool CollidesWith(Block other)
    {
        return
            this.x < other.x + other.width && other.x < this.x + this.width
                && this.y < other.y + other.height && other.y < this.y + this.height;
    }

    public Block WithX(int x)
    {
        return new Block
        {
            tag = this.tag,
            x = x,
            y = this.y,
            width = this.width,
            height = this.height
        };
    }

    public Block WithY(int y)
    {
        return new Block
        {
            tag = this.tag,
            x = this.x,
            y = y,
            width = this.width,
            height = this.height
        };
    }
}
