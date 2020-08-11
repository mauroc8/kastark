using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public struct PlayerResources
{
    public int plastic;
    public int wood;
    public int organic;

    public PlayerResources(int plastic, int wood, int organic)
    {
        this.plastic = plastic;
        this.wood = wood;
        this.organic = organic;
    }

    public static PlayerResources Empty =>
        new PlayerResources
        {
            plastic = 0,
            wood = 0,
            organic = 0
        };

    public bool IsGreaterOrEqualThan(PlayerResources other)
    {
        return
            this.plastic >= other.plastic
                && this.wood >= other.wood
                && this.organic >= other.organic
            ;
    }

    public PlayerResources Minus(PlayerResources other)
    {
        return
            new PlayerResources
            {
                plastic =
                    this.plastic - other.plastic,
                wood =
                    this.wood - other.wood,
                organic =
                    this.organic - other.organic
            };
    }

    public PlayerResources Add(PlayerResources other)
    {
        return
            new PlayerResources
            {
                plastic =
                    this.plastic + other.plastic,
                wood =
                    this.wood + other.wood,
                organic =
                    this.organic + other.organic
            };
    }
};

public enum Checkpoint
{
    Initial,
    PuzzleA,
    PuzzleB2,
    PuzzleC,
    Garbage,
    Enemy0,
    Enemy1,
    Enemy2,
    Enemy3,
    Arthur,
    Dhende
};

public enum QuestStatus
{
    None,
    FightArthur,
    GetReward
};

public static class Globals
{
    // --- STATE ---

    public static Checkpoint checkpoint =
        Checkpoint.Dhende;

    public static QuestStatus quest =
        QuestStatus.None;

    public static StateStream<PlayerResources> playerResources =
        new StateStream<PlayerResources>(
            new PlayerResources(8, 8, 8)
        );

    public static StateStream<Item[]> inventory =
        new StateStream<Item[]>(
            InitialInventory
        );

    public static bool[] enemiesAreAlive =
        InitialEnemiesAreAlive;

    public static Block[] garbageBlocks =
        InitialGarbageBlocks;

    public static Block[] puzzleABlocks =
        InitialPuzzleABlocks;

    public static Block[] puzzleB2Blocks =
        InitialPuzzleB2Blocks;

    public static Block[] puzzleCBlocks =
        InitialPuzzleCBlocks;

    // --- INITIAL STATE ---

    public static
    void NewGame()
    {
        PlayerPrefs.SetInt("gameSaved", 0);

        Scenes.battleResult =
            BattleResult.None;

        checkpoint =
            Checkpoint.Initial;

        quest =
            QuestStatus.None;

        playerResources.Value =
            PlayerResources.Empty;

        inventory.Value =
            InitialInventory;

        enemiesAreAlive =
            InitialEnemiesAreAlive;

        garbageBlocks =
            InitialGarbageBlocks;

        puzzleABlocks =
            InitialPuzzleABlocks;

        puzzleB2Blocks =
            InitialPuzzleB2Blocks;

        puzzleCBlocks =
            InitialPuzzleCBlocks;
    }

    // --- LOAD STATE ---

    public static
    void ContinueGame()
    {
        checkpoint =
            (Checkpoint)PlayerPrefs.GetInt("checkpoint");

        quest =
            (QuestStatus)PlayerPrefs.GetInt("quest");

        playerResources.Value =
            new PlayerResources(
                PlayerPrefs.GetInt("playerResources.plastic"),
                PlayerPrefs.GetInt("playerResources.wood"),
                PlayerPrefs.GetInt("playerResources.organic")
            );

        inventory.Value =
            new Item[10]
            {
                ItemFromString(PlayerPrefs.GetString("inventory-0")),
                ItemFromString(PlayerPrefs.GetString("inventory-1")),
                ItemFromString(PlayerPrefs.GetString("inventory-2")),
                ItemFromString(PlayerPrefs.GetString("inventory-3")),
                ItemFromString(PlayerPrefs.GetString("inventory-4")),
                ItemFromString(PlayerPrefs.GetString("inventory-5")),
                ItemFromString(PlayerPrefs.GetString("inventory-6")),
                ItemFromString(PlayerPrefs.GetString("inventory-7")),
                ItemFromString(PlayerPrefs.GetString("inventory-8")),
                ItemFromString(PlayerPrefs.GetString("inventory-9"))
            };

        enemiesAreAlive =
            new bool[]
            {
                1 == PlayerPrefs.GetInt("enemiesAreAlive-0"),
                1 == PlayerPrefs.GetInt("enemiesAreAlive-1"),
                1 == PlayerPrefs.GetInt("enemiesAreAlive-2"),
                1 == PlayerPrefs.GetInt("enemiesAreAlive-3"),
                1 == PlayerPrefs.GetInt("enemiesAreAlive-4")
            };

        garbageBlocks = LoadBlocks("garbage");
        puzzleABlocks = LoadBlocks("puzzleA");
        puzzleB2Blocks = LoadBlocks("puzzleB2");
        puzzleCBlocks = LoadBlocks("puzzleC");
    }

    // --- SAVE STATE ---

    public static
    void Save()
    {
        PlayerPrefs.SetInt("gameSaved", 1);

        PlayerPrefs.SetInt(
            "playerResources.plastic",
            playerResources.Value.plastic
        );

        PlayerPrefs.SetInt(
            "playerResources.wood",
            playerResources.Value.wood
        );

        PlayerPrefs.SetInt(
            "playerResources.organic",
            playerResources.Value.organic
        );

        PlayerPrefs.SetInt(
            "checkpoint",
            (int)checkpoint
        );

        PlayerPrefs.SetInt(
            "quest",
            (int)quest
        );

        PlayerPrefs.SetString("inventory-0", ItemToString(inventory.Value[0]));
        PlayerPrefs.SetString("inventory-1", ItemToString(inventory.Value[1]));
        PlayerPrefs.SetString("inventory-2", ItemToString(inventory.Value[2]));
        PlayerPrefs.SetString("inventory-3", ItemToString(inventory.Value[3]));
        PlayerPrefs.SetString("inventory-4", ItemToString(inventory.Value[4]));
        PlayerPrefs.SetString("inventory-5", ItemToString(inventory.Value[5]));
        PlayerPrefs.SetString("inventory-6", ItemToString(inventory.Value[6]));
        PlayerPrefs.SetString("inventory-7", ItemToString(inventory.Value[7]));
        PlayerPrefs.SetString("inventory-8", ItemToString(inventory.Value[8]));
        PlayerPrefs.SetString("inventory-9", ItemToString(inventory.Value[9]));

        PlayerPrefs.SetInt("enemiesAreAlive-0", enemiesAreAlive[0] ? 1 : 0);
        PlayerPrefs.SetInt("enemiesAreAlive-1", enemiesAreAlive[1] ? 1 : 0);
        PlayerPrefs.SetInt("enemiesAreAlive-2", enemiesAreAlive[2] ? 1 : 0);
        PlayerPrefs.SetInt("enemiesAreAlive-3", enemiesAreAlive[3] ? 1 : 0);
        PlayerPrefs.SetInt("enemiesAreAlive-4", enemiesAreAlive[4] ? 1 : 0);

        SaveBlocks("garbage", garbageBlocks);
        SaveBlocks("puzzleA", puzzleABlocks);
        SaveBlocks("puzzleB2", puzzleB2Blocks);
        SaveBlocks("puzzleC", puzzleCBlocks);
    }

    // --- PUBLIC HELPER FUNCTIONS ---

    public static
    bool HasSavedGame()
    {
        return
            PlayerPrefs.GetInt("gameSaved") == 1;
    }

    public static bool HasQuest =>
        quest != QuestStatus.None;

    // --- SERIALIZATION HELPER FUNCTIONS ---

    private static
    Item ItemFromString(string str)
    {
        var words =
            str.Split(' ');

        switch (words[0])
        {
            case "empty":
                return new Items.Empty();

            case "sword":
                return new Items.Sword();

            case "magic":
                return new Items.Magic();

            case "shield":
                return new Items.Shield();

            case "potion":
                return
                    new Items.Potion(
                        int.Parse(words[1])
                    );

            case "clover":
                return new Items.Clover();
        }

        return new Items.Empty();
    }

    private static
    string ItemToString(Item item)
    {
        switch (item)
        {
            case Items.Empty _:
                return "empty";

            case Items.Sword _:
                return "sword";

            case Items.Magic _:
                return "magic";

            case Items.Shield _:
                return "shield";

            case Items.Potion potion:
                return $"potion {potion.amount}";

            case Items.Clover _:
                return "clover";
        }

        return "empty";
    }

    // --- INITIAL VALUES ---

    private static Item[] FullInventory =>
        new Item[10]
        {
            // Inventory
            new Items.Empty(),
            new Items.Clover(),
            new Items.Empty(),
            new Items.Empty(),
            new Items.Empty(),
            new Items.Empty(),

            // Ability Bar
            new Items.Sword(),
            new Items.Magic(),
            new Items.Shield(),
            new Items.Potion(2)
        };

    private static Item[] InitialInventory =>
        new Item[10]
        {
            new Items.Empty(),
            new Items.Empty(),
            new Items.Empty(),
            new Items.Empty(),
            new Items.Empty(),
            new Items.Empty(),
            new Items.Empty(),
            new Items.Empty(),
            new Items.Empty(),
            new Items.Empty()
        };

    public static bool[] InitialEnemiesAreAlive =>
        new bool[]
        {
            true,
            true,
            true,
            true,
            true
        };

    public static Block[] InitialGarbageBlocks =>
        new Block[]
        {
            Block.WideBottle(0, 0),
            Block.Can(0, 2),
            Block.WideBottle(1, 1),
            Block.Can(2, 2)
        };

    public static Block[] InitialPuzzleABlocks =>
        new Block[]
        {
            Block.PuzzleA(4, 0, 0),
            Block.PuzzleA(2, 0, 1),
            Block.PuzzleA(3, 1, 1),
            Block.PuzzleA(0, 0, 2),
            Block.PuzzleA(1, 1, 2)
        };

    public static Block[] InitialPuzzleB2Blocks =>
        new Block[]
        {
            Block.PuzzleB2(3, 0, 0),
            Block.PuzzleB2(5, 1, 0),
            Block.PuzzleB2(1, 2, 0),
            Block.PuzzleB2(2, 0, 1),
            Block.PuzzleB2(0, 1, 1),
            Block.PuzzleB2(4, 1, 2)
        };

    public static Block[] InitialPuzzleCBlocks =>
        new Block[]
        {
            Block.PuzzleC(1, 1, 0),
            Block.PuzzleC(7, 2, 0),
            Block.PuzzleC(6, 0, 1),
            Block.PuzzleC(2, 1, 1),
            Block.PuzzleC(4, 2, 1),
            Block.PuzzleC(5, 0, 2),
            Block.PuzzleC(0, 1, 2),
            Block.PuzzleC(3, 2, 2)
        };

    public static int PotionAmount
    {
        get
        {
            for (int i = 0; i < 10; i++)
            {
                switch (inventory.Value[i])
                {
                    case Items.Potion potion:
                        return potion.amount;
                }
            }

            return 0;
        }

        set
        {
            for (int i = 0; i < 10; i++)
            {
                switch (inventory.Value[i])
                {
                    case Items.Potion potion:

                        var newInventory =
                            inventory.Value;

                        newInventory[i] =
                            new Items.Potion(value);

                        inventory.Value =
                            newInventory;

                        return;
                }
            }
        }
    }

    public static Block[] LoadBlocks(string blocksName)
    {
        var length =
            PlayerPrefs.GetInt($"{blocksName}-length");

        var blocks =
            new Block[length];

        for (int i = 0; i < length; i++)
        {
            blocks[i] =
                BlockFromString(
                    PlayerPrefs.GetString($"{blocksName}-block-{i}")
                );
        }

        return blocks;
    }

    public static void SaveBlocks(string blocksName, Block[] blocks)
    {
        PlayerPrefs.SetInt($"{blocksName}-length", blocks.Length);

        for (int i = 0; i < blocks.Length; i++)
        {
            PlayerPrefs.SetString($"{blocksName}-block-{i}", BlockToString(blocks[i]));
        }
    }

    public static Block BlockFromString(string str)
    {
        var args =
            str.Split(' ');

        return
            new Block
            {
                x =
                    int.Parse(args[0]),
                y =
                    int.Parse(args[1]),
                width =
                    int.Parse(args[2]),
                height =
                    int.Parse(args[3]),
                tag =
                    BlockTagFromString(args[4])
            };
    }

    public static string BlockToString(Block block)
    {
        return
            $"{block.x} {block.y} {block.width} {block.height} {BlockTagToString(block.tag)}";
    }

    public static BlockTag BlockTagFromString(string str)
    {
        var args =
            str.Split('-');

        switch (args[0])
        {
            case "CanBlock":
                return new CanBlock();

            case "BottleBlock":
                return new BottleBlock();

            case "WideBottleBlock":
                return new WideBottleBlock();

            case "PuzzleABlock":
                return new PuzzleABlock { index = int.Parse(args[1]) };

            case "PuzzleB2Block":
                return new PuzzleB2Block { index = int.Parse(args[1]) };

            case "PuzzleCBlock":
                return new PuzzleCBlock { index = int.Parse(args[1]) };

            default:
                Debug.LogError($"Error loading block tag: '{str}'");

                return default(BlockTag);
        }
    }

    public static string BlockTagToString(BlockTag tag)
    {
        switch (tag)
        {
            case CanBlock _:
                return "CanBlock";

            case BottleBlock _:
                return "BottleBlock";

            case WideBottleBlock _:
                return "WideBottleBlock";

            case PuzzleABlock x:
                return $"PuzzleABlock-{x.index}";

            case PuzzleB2Block x:
                return $"PuzzleB2Block-{x.index}";

            case PuzzleCBlock x:
                return $"PuzzleCBlock-{x.index}";

            default:
                Debug.LogError($"Cannot convert block tag to string: '{tag}'");

                return "ErrorBlock";
        }
    }
}
