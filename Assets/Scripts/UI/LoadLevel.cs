using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;

public class LoadLevel : MonoBehaviour
{
    private TextMeshProUGUI loadLevelPercentage;

    private void Awake()
    {
        loadLevelPercentage = GetComponent<TextMeshProUGUI>();
    }
    
    public void LoadGame(int levelIndex)
    {
        StartCoroutine(LoadSceneAsync(levelIndex));
    }

    public void LoadGame(string levelName)
    {
        StartCoroutine(LoadSceneAsync(levelName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadLevelPercentage.SetText(Mathf.RoundToInt(progress * 100) + "%");

            yield return null;
        }
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadLevelPercentage.SetText(progress * 100 + "%");

            yield return null;
        }
    }
}
