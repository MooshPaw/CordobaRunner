using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LSLevelEntry : MonoBehaviour
{
    public string levelName, displayName;

    [TextArea]
    public string previewDescription;
    public Sprite previewThumbnail;
    public string coursePreviewScene;

    public GameObject mapPointActive, mapPointInactive;

    private bool canLoadLevel, levelLoading;

    public int crystalsRequired;
    private bool levelUnlocked;

    // Buffered the same way as the player's own jump: a press is held for a short window
    // instead of being discarded the instant it's read, so it isn't lost to a stray single-frame
    // gap between the jump press and canLoadLevel/Update() timing (e.g. jumping momentarily
    // lifts the player's collider out of the trigger zone before Update() gets to consume it).
    private const float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private void OnEnable() => PlayerController.JumpPerformed += HandleJumpPerformed;
    private void OnDisable() => PlayerController.JumpPerformed -= HandleJumpPerformed;
    private void HandleJumpPerformed() => jumpBufferCounter = jumpBufferTime;

    // Start is called before the first frame update
    void Start()
    {
        if(crystalsRequired > 0 && LevelManager.instance.currentCrystals < crystalsRequired)
        {
            levelUnlocked = false;
        } else
        {
            levelUnlocked = true;
        }

        mapPointActive.SetActive(levelUnlocked);
        mapPointInactive.SetActive(!levelUnlocked);

        if (PlayerPrefs.GetString("CurrentLevel") == levelName)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            player.charCon.Move(transform.position - player.transform.position);

            FindObjectOfType<CameraController>().SnapToTarget();

            LevelManager.instance.respawnPoint = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpBufferCounter > 0f) jumpBufferCounter -= Time.deltaTime;

        if(canLoadLevel && levelUnlocked && jumpBufferCounter > 0f && !levelLoading)
        {
            levelLoading = true;
            jumpBufferCounter = 0f;
            StartCoroutine(LoadLevelCo());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            canLoadLevel = true;

            LevelSelectController.instance.SetLevelInfoBoxActive(true);
            LevelSelectController.instance.SetLevelText(displayName);

            if(levelUnlocked)
            {
                LevelSelectController.instance.SetActionText("Press Jump To Enter");
            } else
            {
                LevelSelectController.instance.SetActionText(crystalsRequired.ToString() + " Crystals required to unlock");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            canLoadLevel = false;

            LevelSelectController.instance.SetLevelInfoBoxActive(false);
        }
    }

    public IEnumerator LoadLevelCo()
    {
        FindObjectOfType<PlayerController>().stopMoving = true;
        UIController.instance.FadeToBlack();

        AudioManager.instance.PlaySFX(9);

        yield return new WaitForSeconds(2f);

        PendingLevel.levelName = levelName;
        PendingLevel.displayName = displayName;
        PendingLevel.description = previewDescription;
        PendingLevel.thumbnail = previewThumbnail;

        SceneManager.LoadScene(coursePreviewScene);
    }
}
