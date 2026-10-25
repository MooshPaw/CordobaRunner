using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public string levelToLoad, displayName;

    [TextArea]
    public string previewDescription;
    public Sprite previewThumbnail;

    [Tooltip("Scene that shows the next level's info before it starts. Leave empty to load levelToLoad directly instead.")]
    public string coursePreviewScene = "CoursePreview";

    private bool exiting;

    public Animator anim;

    public Transform camPos;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayMusic(2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(!exiting)
            {
                exiting = true;
                if (anim != null) anim.SetBool("active", true);

                LevelManager.instance.EndLevel();

                FindObjectOfType<CameraController>().endCamPos = camPos;

                AudioManager.instance.PlayMusic(4);

                AudioManager.instance.PlaySFX(8);

                StartCoroutine(FadeOutAndLoad());
            }
        }
    }

    // Gives the player's end-of-level animation time to play, then hands off to
    // FadeScreen's Animator - the actual scene load happens from its "Fade Out"
    // animation's completion event, not on a fixed timer.
    private IEnumerator FadeOutAndLoad()
    {
        yield return new WaitForSeconds(LevelManager.instance.waitToEndLevel - 1f);

        string sceneToLoad = levelToLoad;

        if (!string.IsNullOrEmpty(coursePreviewScene))
        {
            PendingLevel.levelName = levelToLoad;
            PendingLevel.displayName = displayName;
            PendingLevel.description = previewDescription;
            PendingLevel.thumbnail = previewThumbnail;

            sceneToLoad = coursePreviewScene;
        }

        Debug.Log($"[LevelExit:{name}] levelToLoad='{levelToLoad}' coursePreviewScene='{coursePreviewScene}' -> sceneToLoad='{sceneToLoad}', PendingLevel.levelName='{PendingLevel.levelName}'");

        FindObjectOfType<FadeScreen>(true).PlayFadeOut(sceneToLoad);
    }
}
