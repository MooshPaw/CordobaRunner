using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Peque\u00f1o popup propio, separado del UIController compartido a prop\u00f3sito
// (para no tocar ese script y arriesgar nada con el modo Runner).
public class PhotoNotificationUI : MonoBehaviour
{
    public GameObject notificationPanel;
    public TMP_Text notificationText;
    public float displayTime = 2f;

    public void ShowNotification(string subjectName, AlbumCategory category)
    {
        StopAllCoroutines();
        StartCoroutine(ShowCo(subjectName, category));
    }

    private IEnumerator ShowCo(string subjectName, AlbumCategory category)
    {
        notificationPanel.SetActive(true);
        notificationText.text = "¡Foto capturada! " + subjectName + " (" + category.ToString() + ")";

        yield return new WaitForSeconds(displayTime);

        notificationPanel.SetActive(false);
    }
}
