using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public enum BattleStatus
{
    NotPlayed,
    Lost,
    Won
}

public struct PlayerResources
{
    public int plastic;
    public int wood;
    public int organic;
};

public enum Checkpoint
{
    Initial,
    FirstMatch3,
    SecondMatch3,
    Dwarf,
    SecondFight,
    FirstFight
};

public enum GameProgress
{
    Initial,
    DefeatedOthok,
    DefeatedArthur
};

public static class Globals
{
    public static void MoveToScene(int direction)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + direction);
    }

    public static void SetScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public static StateStream<bool> hasQuest =
        new StateStream<bool>(true);

    public static StateStream<bool> hasSword =
        new StateStream<bool>(true);

    public static StateStream<bool> hasShield =
        new StateStream<bool>(true);

    public static StateStream<bool> hasMagic =
        new StateStream<bool>(true);

    public static StateStream<int> potions =
        new StateStream<int>(2);

    public static GameProgress progress =
        GameProgress.Initial;

    public static bool ShowOthokDialog =>
        progress == GameProgress.DefeatedOthok && checkpoint == Checkpoint.FirstFight;

    public static StateStream<PlayerResources> playerResources =
        new StateStream<PlayerResources>(
            new PlayerResources
            {
                plastic = 0,
                wood = 0,
                organic = 0
            }
        );

    private static Dictionary<BoardTag, Block[]> InitialBoards =>
        new Dictionary<BoardTag, Block[]>
        {
            {
                BoardTag.FirstBoard,
                new Block[]
                {
                    Block.Organic, Block.Wood, Block.Wood,
                    Block.Wood, Block.Plastic, Block.Organic,
                    Block.Plastic, Block.Organic, Block.Plastic
                }
            },
            {
                BoardTag.SecondBoard,
                new Block[]
                {
                    Block.Plastic, Block.Organic, Block.Wood,
                    Block.Plastic, Block.Organic, Block.Wood,
                    Block.Organic, Block.Wood, Block.Plastic
                }
            }
        };

    public static Dictionary<BoardTag, Block[]> boards =
        InitialBoards;

    public static Checkpoint checkpoint =
        Checkpoint.FirstMatch3;

    public static
    void Save(Checkpoint c)
    {
        checkpoint = c;

        PlayerPrefs.SetInt("gameSaved", 1);

        PlayerPrefs.SetInt("hasQuest", hasQuest.Value ? 1 : 0);
        PlayerPrefs.SetInt("hasMagic", hasMagic.Value ? 1 : 0);
        PlayerPrefs.SetInt("hasSword", hasSword.Value ? 1 : 0);
        PlayerPrefs.SetInt("hasShield", hasShield.Value ? 1 : 0);
        PlayerPrefs.SetInt("potions", potions.Value);
        PlayerPrefs.SetInt("playerResources.plastic", playerResources.Value.plastic);
        PlayerPrefs.SetInt("playerResources.wood", playerResources.Value.wood);
        PlayerPrefs.SetInt("playerResources.organic", playerResources.Value.organic);

        PlayerPrefs.SetInt("checkpoint", (int)checkpoint);
        PlayerPrefs.SetInt("progress", (int)progress);

        Block[] board;

        if (!boards.TryGetValue(BoardTag.FirstBoard, out board))
        {
            PlayerPrefs.SetInt("gameSaved", 0);
            return;
        }

        PlayerPrefs.SetString("firstBoard", StringifyBoard(board));

        if (!boards.TryGetValue(BoardTag.SecondBoard, out board))
        {
            PlayerPrefs.SetInt("gameSaved", 0);
            return;
        }

        PlayerPrefs.SetString("secondBoard", StringifyBoard(board));
    }

    public static
    bool HasSavedGame()
    {
        return
            PlayerPrefs.GetInt("gameSaved") == 1;
    }

    public static
    void Load()
    {
        hasQuest.Value =
            PlayerPrefs.GetInt("hasQuest") == 1;
        hasMagic.Value =
            PlayerPrefs.GetInt("hasMagic") == 1;
        hasSword.Value =
            PlayerPrefs.GetInt("hasSword") == 1;
        hasShield.Value =
            PlayerPrefs.GetInt("hasShield") == 1;
        potions.Value =
            PlayerPrefs.GetInt("potions");

        playerResources.Value =
            new PlayerResources
            {
                plastic =
                    PlayerPrefs.GetInt("playerResources.plastic"),
                wood =
                    PlayerPrefs.GetInt("playerResources.wood"),
                organic =
                    PlayerPrefs.GetInt("playerResources.organic")
            };

        checkpoint =
            (Checkpoint)PlayerPrefs.GetInt("checkpoint");

        progress =
            (GameProgress)PlayerPrefs.GetInt("progress");

        if (boards.ContainsKey(BoardTag.FirstBoard))
            boards.Remove(BoardTag.FirstBoard);

        boards.Add(
            BoardTag.FirstBoard,
            ParseBoard(
                PlayerPrefs.GetString("firstBoard")
            )
        );

        if (boards.ContainsKey(BoardTag.SecondBoard))
            boards.Remove(BoardTag.SecondBoard);

        boards.Add(
            BoardTag.SecondBoard,
            ParseBoard(
                PlayerPrefs.GetString("secondBoard")
            )
        );

    }

    public static
    void Reset()
    {
        PlayerPrefs.SetInt("gameSaved", 0);

        hasQuest.Value = false;
        hasMagic.Value = true;
        hasSword.Value = false;
        hasShield.Value = false;
        potions.Value = 0;
        playerResources.Value = new PlayerResources { plastic = 0, wood = 0, organic = 0 };
        boards = InitialBoards;
        checkpoint = Checkpoint.Initial;
        progress = GameProgress.Initial;
    }

    private static
    string StringifyBoard(Block[] board)
    {
        return
            string.Join(
                ",",
                board
                    .Select(block => $"{(int)block}")
            );
    }

    private static
    Block[] ParseBoard(string text)
    {
        return
            text
                .Split(',')
                .Select(blockNumber => (Block)int.Parse(blockNumber))
                .ToArray();
    }
}
