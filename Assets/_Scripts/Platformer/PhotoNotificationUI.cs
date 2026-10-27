using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necesario para los textos modernos de Unity

public class PhotoNotificationUI : MonoBehaviour
{
    [Header("Elementos Visuales")]
    [Tooltip("El panel principal que contiene el fondo, la imagen y los textos")]
    public GameObject cardPanel; 
    public Image photoImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    [Header("Configuración")]
    public float showTime = 4f; // Segundos que dura la foto en pantalla

    void Start()
    {
        // Apagamos la tarjeta al inicio del nivel para que no estorbe
        if (cardPanel != null) cardPanel.SetActive(false);
    }

    // Esta función recibe los datos y los pone en la pantalla
    public void ShowPhotoCard(PhotoEntryData data)
    {
        if (data == null) return;

        // 1. Asignar los datos a la interfaz
        nameText.text = data.displayName;
        descriptionText.text = data.description;
        photoImage.sprite = data.photo;

        // 2. Encender el panel
        cardPanel.SetActive(true);

        // 3. Programar que se apague solo después de unos segundos
        CancelInvoke("HidePhotoCard");
        Invoke("HidePhotoCard", showTime);
    }

    private void HidePhotoCard()
    {
        cardPanel.SetActive(false);
    }

    // Wired to the card's close (X) button, so the player isn't forced to wait
    // out the auto-close timer.
    public void CloseCard()
    {
        CancelInvoke("HidePhotoCard");
        HidePhotoCard();
    }
}