using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelIcon : MonoBehaviour
{
    public GameObject levelList;
    public Image previewImage;
    public TextMeshProUGUI descriptionObject;
    public string description;
    public Sprite preview;
    
    public void LoadGame(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadGame(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void LoadDescription()
    {
        previewImage.gameObject.SetActive(true);
        previewImage.sprite = preview;
        descriptionObject.gameObject.SetActive(true);
        descriptionObject.text = description;
    }

    public void Back()
    {
        levelList.SetActive(true);

        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
