using UnityEngine.SceneManagement;

public static class WorldBattleCommunication
{
    public static bool hasWonBattle = false;
    public static bool hasEnteredBattle = false;

    public static void Reset()
    {
        hasWonBattle = false;
        hasEnteredBattle = false;
    }

    public static void MoveToScene(int direction)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + direction);
    }

    public static void AiraLostBattle()
    {
        hasWonBattle = false;
        hasEnteredBattle = true;
        MoveToScene(-1);
    }

    public static void OthokLostBattle()
    {
        hasWonBattle = true;
        hasEnteredBattle = true;
        MoveToScene(-1);
    }
}
