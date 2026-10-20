using UnityEngine;

public enum AlbumCategory 
{
    Animal,
    Comida,
    Lugar,
    Otro
}

[CreateAssetMenu(fileName = "NuevoElemento", menuName = "Juego Cordoba/Nueva Entrada de Foto")]
public class PhotoEntryData : ScriptableObject
{
    [Header("Datos de la Foto")]
    public string displayName = "Nuevo Objeto"; // Renombrado para coincidir con tu UI
    public AlbumCategory categoria;
    
    [TextArea(3, 5)]
    public string description; // Renombrado para coincidir con tu UI
    
    [Header("Imagen")]
    public Sprite photo; // Agregado para solucionar el error de la línea 35
}