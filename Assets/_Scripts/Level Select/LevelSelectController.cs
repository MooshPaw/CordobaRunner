using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Lives on a single always-active UIManager object (not under either the PC or
/// Vertical canvas) so Awake() only ever runs once per scene. Every widget field is
/// an array with one slot per canvas variant (PC, Vertical) and every update loops
/// over all of them, so both canvases stay in sync regardless of which one is
/// currently visible.
/// </summary>
public class LevelSelectController : MonoBehaviour
{
    public static LevelSelectController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject[] levelInfoBox;
    public TMP_Text[] levelText, actionText;

    private void Start()
    {
        AudioManager.instance.PlayMusic(1);
    }

    public void SetLevelInfoBoxActive(bool active)
    {
        foreach (var box in levelInfoBox) box.SetActive(active);
    }

    public void SetLevelText(string text)
    {
        foreach (var text32 in levelText) text32.text = text;
    }

    public void SetActionText(string text)
    {
        foreach (var text32 in actionText) text32.text = text;
    }
}
