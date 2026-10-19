using UnityEngine;

[CreateAssetMenu(fileName = "PhotoCatalog", menuName = "Pulso Ecologico/Photo Catalog")]
public class PhotoCatalog : ScriptableObject
{
    public PhotoEntryData[] entries;
}