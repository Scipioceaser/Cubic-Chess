using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void SelectLevel(GameObject levelObject)
    {
        levelObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Back(GameObject goBackTo)
    {
        goBackTo.SetActive(true);
        gameObject.SetActive(false);
    }
}
