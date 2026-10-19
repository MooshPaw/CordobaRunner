using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Muestra la foto y su descripcion apenas se captura. Debe ir en un objeto que
// siempre este activo (el Canvas), no en el panel que se apaga.
public class PhotoNotificationUI : MonoBehaviour
{
    public GameObject notificationPanel;
    public TMP_Text notificationText;   // titulo: "¡Foto capturada! Nombre"
    public Image photoImage;
    public TMP_Text descriptionText;
    public float displayTime = 4f;

    public void ShowEntry(PhotoEntryData entry)
    {
        StopAllCoroutines();
        StartCoroutine(ShowCo(entry));
    }

    public void Hide()
    {
        StopAllCoroutines();
        notificationPanel.SetActive(false);
    }

    private IEnumerator ShowCo(PhotoEntryData entry)
    {
        if (notificationText != null) notificationText.text = "¡Foto capturada! " + entry.displayName;
        if (descriptionText != null) descriptionText.text = entry.description;

        if (photoImage != null)
        {
            photoImage.sprite = entry.photo;
            photoImage.color = Color.white;
        }

        notificationPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(displayTime);

        notificationPanel.SetActive(false);
    }
}