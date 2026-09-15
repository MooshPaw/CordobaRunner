using UnityEngine;

// Puente est\u00e1tico entre escenas: LSLevelEntry lo llena justo antes de cargar
// CoursePreview, y CoursePreviewController lo lee apenas arranca esa escena.
public static class PendingLevel
{
    public static string levelName;
    public static string displayName;
    public static string description;
    public static Sprite thumbnail;
}
