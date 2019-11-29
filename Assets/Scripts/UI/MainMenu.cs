using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsMenu;

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
        settingsMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
