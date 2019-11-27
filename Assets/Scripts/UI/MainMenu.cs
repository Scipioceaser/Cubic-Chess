using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void NewGame(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void Settings()
    {
        
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
