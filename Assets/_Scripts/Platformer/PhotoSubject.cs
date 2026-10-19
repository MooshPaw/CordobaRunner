using System.Collections.Generic;
using UnityEngine;

public enum AlbumCategory
{
    Naturaleza,
    Territorio,
    VidaRural,
    Cultura,
    Gastronomia
}

// Va en cada objeto del nivel que se pueda fotografiar. Nombre, descripcion e
// imagen viven en el PhotoEntryData que le asignes.
public class PhotoSubject : MonoBehaviour
{
    public PhotoEntryData entry;

    [HideInInspector]
    public bool photographed;

    // Lista de sujetos activos: evita FindObjectsOfType en cada frame (importante en celular).
    public static readonly List<PhotoSubject> All = new List<PhotoSubject>();

    private void OnEnable() { All.Add(this); }
    private void OnDisable() { All.Remove(this); }

    private void Start()
    {
        if (entry == null)
        {
            Debug.LogWarning("[PhotoSubject] " + name + " no tiene 'Entry' asignado.");
        }

        Refresh();
    }

    public void Refresh()
    {
        photographed = entry == null || AlbumManager.IsDiscovered(entry);
    }

    public void MarkPhotographed()
    {
        if (photographed) return;

        if (AlbumManager.instance == null)
        {
            Debug.LogWarning("[PhotoSubject] No hay AlbumManager en la escena (falta el prefab PhotoSystem).");
            return;
        }

        AlbumManager.instance.RegisterPhoto(entry);
    }
}
