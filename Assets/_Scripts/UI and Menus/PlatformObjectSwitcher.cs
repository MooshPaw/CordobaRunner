using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

/// <summary>
/// Enables/disables GameObjects depending on the platform the project is compiled for.
/// Put this on any object in your scene and drag objects into the lists.
///
///  - PC only      : active on Standalone (Windows/Mac/Linux) and Editor with a PC build target
///  - Android only : active when the build target is Android
///
/// It is applied:
///  1. At runtime (Awake)
///  2. In the editor whenever scripts reload / you switch build target
///  3. During a build (scene processing), so the built scene is always correct
/// </summary>
[DisallowMultipleComponent]
public class PlatformObjectSwitcher : MonoBehaviour
{
    [Tooltip("Enabled on PC, disabled on Android")]
    public GameObject[] pcOnly;

    [Tooltip("Enabled on Android, disabled on PC")]
    public GameObject[] androidOnly;

    public static bool IsAndroid
    {
        get
        {
#if UNITY_ANDROID
            return true;
#else
            return false;
#endif
        }
    }

    private void Awake() => Apply();

    [ContextMenu("Apply For Current Platform")]
    public bool Apply()
    {
        bool android = IsAndroid;
        bool changed = false;
        changed |= SetActive(pcOnly, !android);
        changed |= SetActive(androidOnly, android);
        return changed;
    }

    private static bool SetActive(GameObject[] objects, bool active)
    {
        bool changed = false;
        if (objects == null) return false;

        foreach (var go in objects)
        {
            if (go == null || go.activeSelf == active) continue;
            go.SetActive(active);
#if UNITY_EDITOR
            if (!Application.isPlaying) EditorUtility.SetDirty(go);
#endif
            changed = true;
        }
        return changed;
    }
}

#if UNITY_EDITOR
/// <summary>Re-applies the switchers in open scenes after a recompile / build target switch.</summary>
[InitializeOnLoad]
static class PlatformObjectSwitcherEditor
{
    static PlatformObjectSwitcherEditor()
    {
        EditorApplication.delayCall += ApplyToOpenScenes;
    }

    static void ApplyToOpenScenes()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        foreach (var switcher in Object.FindObjectsByType<PlatformObjectSwitcher>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (switcher.Apply())
                EditorSceneManager.MarkSceneDirty(switcher.gameObject.scene);
        }
    }
}

/// <summary>Makes sure scenes included in a build have the right objects active.</summary>
class PlatformObjectSwitcherBuild : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        foreach (var root in scene.GetRootGameObjects())
            foreach (var switcher in root.GetComponentsInChildren<PlatformObjectSwitcher>(true))
                switcher.Apply();
    }
}
#endif
