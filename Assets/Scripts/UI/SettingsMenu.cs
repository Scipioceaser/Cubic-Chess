using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    public void Volume(float volume)
    {
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }
    
    public void Fullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void Back(GameObject objectToGoBackTo)
    {
        objectToGoBackTo.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SettingLow()
    {
        QualitySettings.SetQualityLevel(0);
    }

    public void SettingMedium()
    {
        QualitySettings.SetQualityLevel(1);
    }

    public void SettingHigh()
    {
        QualitySettings.SetQualityLevel(2);
    }
}
