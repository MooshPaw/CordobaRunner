using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotoJournalSlot : MonoBehaviour
{
    public Image photoImage;
    public TMP_Text nameText;
    public Button button;

    public void Setup(PhotoEntryData entry, bool found, PhotoJournalUI journal)
    {
        photoImage.sprite = entry.photo;
        photoImage.color = found ? Color.white : Color.black;   // negro hasta fotografiarlo
        nameText.text = found ? entry.displayName : "???";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => journal.ShowDetail(entry, found));
    }
}