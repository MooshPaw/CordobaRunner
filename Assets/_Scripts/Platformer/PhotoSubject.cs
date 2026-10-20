using UnityEngine;

public class PhotoSubject : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Arrastra aquí el archivo de datos (PhotoEntryData) de este objeto")]
    public PhotoEntryData entry; 

    [HideInInspector]
    public bool isPhotographed = false;

    public void MarkAsPhotographed()
    {
        if (!isPhotographed && entry != null)
        {
            isPhotographed = true;
            // Aquí está la corrección: ahora usa "displayName"
            Debug.Log($"¡Foto tomada exitosamente a: {entry.displayName}!");
            
        }
        else if (entry == null)
        {
            Debug.LogWarning("¡A este objeto le falta que le arrastres su archivo Entry en el Inspector!");
        }
    }
}