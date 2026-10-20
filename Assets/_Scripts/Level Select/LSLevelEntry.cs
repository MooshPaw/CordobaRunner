using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
        if(canLoadLevel && !levelLoading && JumpPressed())
        {
            levelLoading = true;
            StartCoroutine(LoadLevelCo());
        }
    }

    // Detecta el salto con el sistema viejo y con el Input System:
    // barra espaciadora, boton A de un control, y el boton de salto en pantalla
    // (el OnScreenButton aparece como un control virtual "Button South").
    private bool JumpPressed()
    {
        bool pressed = false;

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetButtonDown("Jump")) pressed = true;
#endif

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) pressed = true;
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) pressed = true;
#endif

        return pressed;
    }

        private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            canLoadLevel = true;

            string message = levelUnlocked
                ? "Press Jump To Enter"
                : crystalsRequired.ToString() + " Crystals required to unlock";

            foreach (var box in LevelSelectController.instance.levelInfoBox)
            {
                if (box != null) box.SetActive(true);
            }

            foreach (var t in LevelSelectController.instance.levelText)
            {
                if (t != null) t.text = displayName;
            }

            foreach (var t in LevelSelectController.instance.actionText)
            {
                if (t != null) t.text = message;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            canLoadLevel = false;

            foreach (var box in LevelSelectController.instance.levelInfoBox)
            {
                if (box != null) box.SetActive(false);
            }
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

        // Si se te olvido llenar 'Course Preview Scene', carga el nivel directo
        // en vez de dar el error de "Invalid scene name".
        if (string.IsNullOrEmpty(coursePreviewScene))
        {
            SceneManager.LoadScene(levelName);
            PlayerPrefs.SetString("CurrentLevel", levelName);
        }
        else
        {
            SceneManager.LoadScene(coursePreviewScene);
        }
    }
}