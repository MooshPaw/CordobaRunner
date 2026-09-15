using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CoursePreviewController : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Image thumbnailImage;
    public string levelSelectScene;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        titleText.text = PendingLevel.displayName;
        descriptionText.text = PendingLevel.description;

        if (PendingLevel.thumbnail != null)
        {
            thumbnailImage.sprite = PendingLevel.thumbnail;
        }
    }

    public void ConfirmAndPlay()
    {
        SceneManager.LoadScene(PendingLevel.levelName);
        PlayerPrefs.SetString("CurrentLevel", PendingLevel.levelName);
    }

    public void Back()
    {
        SceneManager.LoadScene(levelSelectScene);
    }
}
