using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Each widget field is an array with one slot per canvas variant (PC, Vertical) so both
/// canvases show the pending level's info, regardless of which one is currently visible.
/// </summary>
public class CoursePreviewController : MonoBehaviour
{
    public TMP_Text[] titleText;
    public TMP_Text[] descriptionText;
    public Image[] thumbnailImage;
    public string levelSelectScene;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (var text in titleText) text.text = PendingLevel.displayName;
        foreach (var text in descriptionText) text.text = PendingLevel.description;

        if (PendingLevel.thumbnail != null)
        {
            foreach (var image in thumbnailImage) image.sprite = PendingLevel.thumbnail;
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
