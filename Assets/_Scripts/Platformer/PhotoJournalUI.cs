using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotoJournalUI : MonoBehaviour
{
    public PhotoCatalog catalog;

    [Header("Bitacora")]
    public GameObject journalPanel;
    public Transform slotContainer;
    public PhotoJournalSlot slotPrefab;
    public TMP_Text progressText;

    [Header("Detalle")]
    public GameObject detailPanel;
    public Image detailImage;
    public TMP_Text detailTitle;
    public TMP_Text detailCategory;
    public TMP_Text detailDescription;

    private bool isOpen;
    private float previousTimeScale = 1f;
    private CursorLockMode previousLockState;
    private bool previousCursorVisible;

    public void Toggle()
    {
        if (isOpen) Close(); else Open();
    }

    public void Open()
    {
        if (isOpen) return;

        if (catalog == null || slotPrefab == null || slotContainer == null)
        {
            Debug.LogWarning("[PhotoJournalUI] Falta asignar catalog, slotPrefab o slotContainer.");
            return;
        }

        isOpen = true;
        previousTimeScale = Time.timeScale;
        previousLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        detailPanel.SetActive(false);
        journalPanel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        journalPanel.SetActive(false);
        detailPanel.SetActive(false);

        Time.timeScale = previousTimeScale;
        Cursor.lockState = previousLockState;
        Cursor.visible = previousCursorVisible;
    }

    public void CloseDetail()
    {
        detailPanel.SetActive(false);
    }

    private void Refresh()
    {
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        int found = 0;

        foreach (PhotoEntryData entry in catalog.entries)
        {
            if (entry == null) continue;

            bool discovered = AlbumManager.IsDiscovered(entry);
            if (discovered) found++;

            PhotoJournalSlot slot = Instantiate(slotPrefab, slotContainer);
            slot.Setup(entry, discovered, this);
        }

        if (progressText != null)
        {
            progressText.text = found + " / " + catalog.entries.Length;
        }
    }

    public void ShowDetail(PhotoEntryData entry, bool found)
    {
        detailPanel.SetActive(true);

        detailImage.sprite = entry.photo;
        detailImage.color = found ? Color.white : Color.black;

        detailTitle.text = found ? entry.displayName : "???";
        detailCategory.text = entry.category.ToString();
        detailDescription.text = found ? entry.description : "Aún no has fotografiado esto. ¡Sigue explorando!";
    }
}