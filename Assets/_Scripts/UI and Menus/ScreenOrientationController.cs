using UnityEngine;

// Drop one of these into a scene and set `orientation` in the Inspector to lock
// that scene's screen orientation on Android. Desktop builds/the Editor are left
// alone - Screen.orientation has no effect there anyway.
//
// `verticalForPendingLevel` is an optional override for scenes reused across
// multiple pending levels (e.g. CoursePreview): if set and it matches
// PendingLevel.levelName, Vertical is forced regardless of `orientation`.
public class ScreenOrientationController : MonoBehaviour
{
    public enum Orientation { Horizontal, Vertical }

    public Orientation orientation = Orientation.Horizontal;

    public string verticalForPendingLevel;

    // Exposed so other scripts (e.g. VerticalCoursePreviewSwitcher) can ask "should this
    // scene currently be showing its vertical layout" without duplicating the check, and
    // without being tied to the Android-only Screen.orientation call below.
    public Orientation EffectiveOrientation =>
        !string.IsNullOrEmpty(verticalForPendingLevel) && PendingLevel.levelName == verticalForPendingLevel
            ? Orientation.Vertical
            : orientation;

    private void Awake()
    {
        if (Application.platform != RuntimePlatform.Android) return;

        Screen.orientation = EffectiveOrientation == Orientation.Horizontal
            ? ScreenOrientation.LandscapeLeft
            : ScreenOrientation.Portrait;
    }
}
