using UnityEngine;

public class PhotoSubject : MonoBehaviour
{
    [Header("Configuración")]
    public PhotoEntryData entry; 

    public void MarkAsPhotographed()
    {
        // Revisamos la memoria del archivo directamente
        if (entry != null && !entry.isDiscovered) 
        {
            entry.isDiscovered = true; // ¡Se guarda en el archivo y sobrevive al nivel!
            Debug.Log($"¡Foto tomada exitosamente a: {entry.displayName}!");
        }
        else if (entry == null)
        {
            Debug.LogWarning("¡A este objeto le falta que le arrastres su archivo Entry en el Inspector!");
        }
    }
}