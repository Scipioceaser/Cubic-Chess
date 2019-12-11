using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelIcon : MonoBehaviour
{
    public GameObject levelList;
    public Image previewImage;
    public TextMeshProUGUI descriptionObject;
    public string description;
    public Sprite preview;
    public GameObject playButton;
    
    public void LoadDescription()
    {
        previewImage.gameObject.SetActive(true);
        previewImage.sprite = preview;
        descriptionObject.gameObject.SetActive(true);
        descriptionObject.text = description;
        playButton.SetActive(true);
    }

    public void Back()
    {
        levelList.SetActive(true);
        playButton.SetActive(false);

        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
