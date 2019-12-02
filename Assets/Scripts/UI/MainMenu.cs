using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject levelSelectMenu;
    
    public void NewGame()
    {
        levelSelectMenu.SetActive(true);
        gameObject.SetActive(false);
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
