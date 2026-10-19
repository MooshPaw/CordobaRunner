using UnityEngine;

public class AlbumManager : MonoBehaviour
{
    public static AlbumManager instance;

    public PhotoNotificationUI notificationUI;
    public GameObject photoPromptIcon;

    private void Awake()
    {
        instance = this;
    }

    public void RegisterPhoto(PhotoEntryData entry)
    {
        if (entry == null || IsDiscovered(entry)) return;

        PlayerPrefs.SetInt(DiscoveryKey(entry), 1);
        PlayerPrefs.SetInt(CategoryKey(entry.category), GetCount(entry.category) + 1);
        PlayerPrefs.Save();

        // Si hay varios objetos con la misma entrada, todos quedan como ya fotografiados.
        foreach (PhotoSubject subject in PhotoSubject.All)
        {
            subject.Refresh();
        }

        if (notificationUI != null)
        {
            notificationUI.ShowEntry(entry);
        }
    }

    public void SetPhotoPrompt(bool visible)
    {
        if (photoPromptIcon != null && photoPromptIcon.activeSelf != visible)
        {
            photoPromptIcon.SetActive(visible);
        }
    }

    // Lo llama el boton de foto en pantalla (sirve en PC y en celular).
    public void TakePhotoButton()
    {
        if (PhotographyController.instance != null)
        {
            PhotographyController.instance.TakePhoto();
        }
    }

    public static bool IsDiscovered(PhotoEntryData entry)
    {
        return entry != null && PlayerPrefs.GetInt(DiscoveryKey(entry), 0) == 1;
    }

    public int GetCount(AlbumCategory category)
    {
        return PlayerPrefs.GetInt(CategoryKey(category), 0);
    }

    private static string DiscoveryKey(PhotoEntryData entry) { return "Photo_" + entry.Key; }
    private static string CategoryKey(AlbumCategory category) { return "Album_" + category.ToString(); }
}