using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Lives on a single always-active UIManager object (not under either the PC or
/// Vertical canvas) so Awake() only ever runs once per scene. Every widget field is
/// an array with one slot per canvas variant (PC, Vertical) and every update loops
/// over all of them, so both canvases stay in sync regardless of which one is
/// currently visible.
/// </summary>
public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    public Image[] fadeScreen;
    private bool isFadingToBlack, isFadingFromBlack;
    public float fadeSpeed = 2f;

    public Slider[] healthSlider;
    public TMP_Text[] healthText, timeText;

    public TMP_Text[] coinText, crystalText;

    public GameObject[] pauseScreen;

    public string mainMenu, levelSelect;

    // Update is called once per frame
    void Update()
    {
        if(isFadingToBlack)
        {
            foreach (var screen in fadeScreen)
            {
                screen.color = new Color(screen.color.r, screen.color.g, screen.color.b,
                    Mathf.MoveTowards(screen.color.a, 1f, fadeSpeed * Time.deltaTime)
                    );
            }
        }

        if(isFadingFromBlack)
        {
            foreach (var screen in fadeScreen)
            {
                screen.color = new Color(screen.color.r, screen.color.g, screen.color.b,
                    Mathf.MoveTowards(screen.color.a, 0f, fadeSpeed * Time.deltaTime)
                    );
            }
        }
    }

    public void FadeToBlack()
    {
        isFadingToBlack = true;
        isFadingFromBlack = false;
    }

    public void FadeFromBlack()
    {
        isFadingToBlack = false;
        isFadingFromBlack = true;
    }

    public void UpdateHealthDisplay(int health)
    {
        string text = "Health: " + health + "/" + PlayerHealthController.instance.maxHealth;

        foreach (var text32 in healthText) text32.text = text;

        foreach (var slider in healthSlider)
        {
            slider.maxValue = PlayerHealthController.instance.maxHealth;
            slider.value = health;
        }
    }

    public void SetCoinText(string text)
    {
        if (coinText == null) return;
        foreach (var text32 in coinText) text32.text = text;
    }

    public void SetCrystalText(string text)
    {
        if (crystalText == null) return;
        foreach (var text32 in crystalText) text32.text = text;
    }

    public void SetTimeText(string text)
    {
        foreach (var text32 in timeText) text32.text = text;
    }

    public void PauseUnpause()
    {
        bool active = !pauseScreen[0].activeSelf;

        foreach (var screen in pauseScreen) screen.SetActive(active);

        if(active)
        {
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0f;
        } else
        {
            Cursor.lockState = CursorLockMode.Locked;

            Time.timeScale = 1f;
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenu);
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(levelSelect);
    }
}
