using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToNextSceneController : MonoBehaviour
{
    public void MoveToNextScene()
    {
        MoveToScene(1);
    }

    public void MoveToPreviousScene()
    {
        MoveToScene(-1);
    }

    public void MoveToScene(int direction)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + direction);
    }
}
