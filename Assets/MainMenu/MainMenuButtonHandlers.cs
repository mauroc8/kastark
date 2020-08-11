using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonHandlers : MonoBehaviour
{
    public void NewGame()
    {
        Scenes.LoadNewGame();
    }

    public void ChangeLanguage()
    {
        Scenes.ChangeLanguage();
    }

    public void QuitGame()
    {
        Scenes.QuitGame();
    }

    public void Continue()
    {
        Scenes.ContinueGame();
    }
}
