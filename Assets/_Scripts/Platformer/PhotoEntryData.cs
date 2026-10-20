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
    public string displayName = "Nuevo Objeto"; 
    public AlbumCategory categoria;
    
    [TextArea(3, 5)]
    public string description; 
    
    [Header("Imagen")]
    public Sprite photo; 

    [Header("Progreso")]
    [Tooltip("El juego marcará esto automáticamente al tomar la foto")]
    public bool isDiscovered = false; // <-- ESTA ES LA CLAVE PARA PASAR DE NIVEL
}