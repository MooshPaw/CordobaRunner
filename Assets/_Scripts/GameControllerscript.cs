using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

public class StarterController : MonoBehaviour
{
    public string mainMenuScene = "MainMenu";
    public float waitTime = 2f;
    public int targetFrameRate = 60;

    // Nothing in the project ever asserted a frame rate target, and Android's default
    // quality level (Medium) has vSyncCount = 1, which overrides targetFrameRate entirely -
    // so the game was just inheriting whatever clock speed Android happened to boot the
    // display/GPU at. Setting this explicitly, once, as early as possible (this is the
    // first script to run) stops that drift instead of silently going along with it.
    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
        AndroidDevice.SetSustainedPerformanceMode(true);
#endif
    }

    private void Start()
    {
        Invoke(nameof(GoToMainMenu), waitTime);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}