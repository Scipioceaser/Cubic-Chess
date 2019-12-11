using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionsDropdown;
    public Toggle fullscreenToggle;
    private Resolution[] resolutions;

    // Start is called before the first frame update
    private void Start()
    {
        if (resolutionsDropdown != null)
        {
            resolutions = Screen.resolutions;
            resolutionsDropdown.ClearOptions();   

            List<string> options = new List<string>();
            
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";

                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                {
                    currentResolutionIndex = i;
                }
            }
        
            resolutionsDropdown.AddOptions(options);
            resolutionsDropdown.value = currentResolutionIndex;
            resolutionsDropdown.RefreshShownValue();
        }

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = Screen.fullScreen;
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
        if (objectToGoBackTo != null)
        {
            objectToGoBackTo.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Has no menu object to go back to.");
        }
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

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}
