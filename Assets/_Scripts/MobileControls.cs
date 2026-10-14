using UnityEngine;

public class MobileControlsToggle : MonoBehaviour
{
    public bool showInEditor = true;

    void Awake()
    {
        bool show = Application.isMobilePlatform;
#if UNITY_EDITOR
        show = show || showInEditor;
#endif
        gameObject.SetActive(show);
    }
}