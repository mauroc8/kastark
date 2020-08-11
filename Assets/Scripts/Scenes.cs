using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneId
{
    ChangeLanguage,
    MainMenu,
    World,
    Battle
};

public enum BattleResult
{
    Win,
    Lose,
    None
};

public static class Scenes
{
    public static BattleResult battleResult =
        BattleResult.None;

    public static int battleId =
        0;

    public static int BattleId =>
        battleId;

    public static void LoadBattle(int id)
    {
        battleId =
            id;

        LoadScene(SceneId.Battle);
    }

    public static void ReturnFromBattle(BattleResult result)
    {
        battleResult =
            result;

        LoadScene(SceneId.World);
    }


    public static (BattleResult, int) ConsumeBattleResult()
    {
        var result =
            battleResult;

        var id =
            battleId;

        battleResult =
            BattleResult.None;

        battleId =
            0;

        return (result, id);
    }

    public static void LoadMainMenu()
    {
        LoadScene(SceneId.MainMenu);
    }

    static void LoadScene(SceneId sceneId)
    {
        SceneManager.LoadScene((int)sceneId);
    }

    public static void LoadNewGame()
    {
        Globals.NewGame();

        LoadScene(SceneId.World);
    }

    public static void ChangeLanguage()
    {
        LoadScene(SceneId.ChangeLanguage);
    }

    public static void QuitGame()
    {
        if (Globals.HasSavedGame())
            Globals.Save();

        Application.Quit();
    }

    public static void ContinueGame()
    {
        if (!Globals.HasSavedGame())
            return;

        Globals.ContinueGame();

        LoadScene(SceneId.World);
    }
}
