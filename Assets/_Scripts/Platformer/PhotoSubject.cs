using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AlbumCategory
{
    Naturaleza,
    Territorio,
    VidaRural,
    Cultura,
    Gastronomia
}

// Colócalo en cualquier objeto del nivel que Luis Fernando pueda fotografiar
// (flora, huellas, platos t\u00edpicos, sombreros, instrumentos...).
public class PhotoSubject : MonoBehaviour
{
    public string subjectName;
    public AlbumCategory category;

    // Igual que en Crystal.cs: un ID \u00fanico por objeto para que quede guardado
    // como ya fotografiado aunque recargues el nivel.
    public string uniqueID;

    [HideInInspector]
    public bool photographed;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt(PersistenceKey(), 0) == 1)
        {
            photographed = true;
        }
    }

    private string PersistenceKey()
    {
        return SceneManager.GetActiveScene().name + "_photo_" + uniqueID;
    }

    public void MarkPhotographed()
    {
        if (photographed) return;

        photographed = true;
        PlayerPrefs.SetInt(PersistenceKey(), 1);

        AlbumManager.instance.RegisterPhoto(category, subjectName);
    }
}
