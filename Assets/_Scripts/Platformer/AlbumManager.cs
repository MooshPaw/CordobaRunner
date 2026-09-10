using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton igual de estilo que LevelManager/AudioManager. No necesita
// DontDestroyOnLoad porque el conteo se guarda directo en PlayerPrefs,
// as\u00ed que sobrevive aunque el GameObject se destruya al cambiar de escena.
public class AlbumManager : MonoBehaviour
{
    public static AlbumManager instance;

    private void Awake()
    {
        instance = this;
    }

    public PhotoNotificationUI notificationUI; // opcional, puede quedar vac\u00edo

    public void RegisterPhoto(AlbumCategory category, string subjectName)
    {
        int current = PlayerPrefs.GetInt(CategoryKey(category), 0);
        current++;
        PlayerPrefs.SetInt(CategoryKey(category), current);

        if (notificationUI != null)
        {
            notificationUI.ShowNotification(subjectName, category);
        }
    }

    public int GetCount(AlbumCategory category)
    {
        return PlayerPrefs.GetInt(CategoryKey(category), 0);
    }

    private string CategoryKey(AlbumCategory category)
    {
        return "Album_" + category.ToString();
    }
}
