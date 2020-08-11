using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Kastark/BlockDictionary")]
public class BlockDictionary : ScriptableObject
{
    public GameObject canBlock = null;
    public GameObject bottleBlock = null;
    public GameObject wideBottleBlock = null;

    public GameObject[] puzzleABlocks = null;
    public GameObject[] puzzleB2Blocks = null;
    public GameObject[] puzzleCBlocks = null;

    public GameObject GetPrefab(BlockTag block)
    {
        if (Functions.IsTypeOf<BlockTag, PuzzleABlock>(block))
        {
            var puzzleBlock =
                (PuzzleABlock)block;
            return
                puzzleABlocks[puzzleBlock.index];
        }


        if (Functions.IsTypeOf<BlockTag, PuzzleB2Block>(block))
        {
            var puzzleBlock =
                (PuzzleB2Block)block;

            return
                puzzleB2Blocks[puzzleBlock.index];
        }

        if (Functions.IsTypeOf<BlockTag, PuzzleCBlock>(block))
        {
            var puzzleBlock =
                (PuzzleCBlock)block;

            return
                puzzleCBlocks[puzzleBlock.index];
        }

        return
            Functions.Eq(block, new CanBlock()) ? canBlock :
            Functions.Eq(block, new BottleBlock()) ? bottleBlock :
            Functions.Eq(block, new WideBottleBlock()) ? wideBottleBlock :
            null;
    }
}
