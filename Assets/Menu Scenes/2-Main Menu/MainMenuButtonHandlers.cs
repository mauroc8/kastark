using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonHandlers : MonoBehaviour
{
    [SerializeField] MoveToNextSceneController _sceneController = null;
    
    public void NewGame()
    {
        _sceneController.MoveToNextScene();
    }

    public void ChangeLanguage()
    {
        _sceneController.MoveToPreviousScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
