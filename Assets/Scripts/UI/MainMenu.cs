using UnityEngine;
using System.Threading;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject levelSelectMenu;

    private void Start()
    {
        Thread.Sleep(1500);
        settingsMenu.SetActive(false);
    }

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
