using UnityEngine;

[CreateAssetMenu(fileName = "PhotoEntry", menuName = "Pulso Ecologico/Photo Entry")]
public class PhotoEntryData : ScriptableObject
{
    [Tooltip("Opcional. Si lo dejas vacio se usa el nombre del archivo.")]
    public string id;

    public string displayName;
    public AlbumCategory category;

    [TextArea(3, 8)]
    public string description;

    public Sprite photo;

    public string Key
    {
        get { return string.IsNullOrEmpty(id) ? name : id; }
    }
}