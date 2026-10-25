using UnityEngine;

/// <summary>
/// Like PlatformObjectSwitcher, but the condition is "is this CoursePreview showing its
/// vertical layout for the pending level" (per ScreenOrientationController.EffectiveOrientation)
/// instead of the build platform. Put this on any object in the scene and drag the
/// vertical-only objects (e.g. Canvas/LevelDescription (Vertical)) into the list.
///
/// Unlike PlatformObjectSwitcher this only applies at runtime (Awake): the condition
/// depends on PendingLevel.levelName, which is only meaningful once the game has actually
/// navigated into this scene, so there's no "correct" state to bake in at edit/build time.
/// </summary>
public class VerticalCoursePreviewSwitcher : MonoBehaviour
{
    [Tooltip("Enabled only when ScreenOrientationController resolves to Vertical for the pending level")]
    public GameObject[] verticalOnly;

    private void Awake() => Apply();

    public void Apply()
    {
        ScreenOrientationController orientation = FindObjectOfType<ScreenOrientationController>();
        bool vertical = orientation != null && orientation.EffectiveOrientation == ScreenOrientationController.Orientation.Vertical;

        foreach (var go in verticalOnly)
        {
            if (go != null) go.SetActive(vertical);
        }
    }
}
