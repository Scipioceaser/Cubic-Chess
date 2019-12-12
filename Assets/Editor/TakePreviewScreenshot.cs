using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class TakePreviewScreenshot : Editor
{
    [MenuItem("Screenshot/Take Screenshot")]
    static void TakeScreenShot()
    {
        ScreenCapture.CaptureScreenshot("Assets/Images/" + SceneManager.GetActiveScene().name + "_preview.png");
    }
}
