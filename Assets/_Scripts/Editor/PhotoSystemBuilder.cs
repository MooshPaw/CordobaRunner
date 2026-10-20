#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Crea todo el prefab PhotoSystem con un clic:
// menu superior -> Pulso Ecologico -> Crear PhotoSystem
public static class PhotoSystemBuilder
{
    private static readonly Color PanelColor = new Color(0.08f, 0.10f, 0.08f, 0.95f);
    private static readonly Color ButtonColor = new Color(0.20f, 0.45f, 0.25f, 1f);
    private static readonly Color SlotColor = new Color(0.16f, 0.20f, 0.16f, 1f);

    [MenuItem("Pulso Ecologico/Crear PhotoSystem")]
    public static void Build()
    {
        if (Object.FindObjectOfType<AlbumManager>() != null)
        {
            bool create = EditorUtility.DisplayDialog(
                "PhotoSystem",
                "Ya hay un AlbumManager en esta escena. ¿Crear otro PhotoSystem de todas formas?",
                "Crear otro",
                "Cancelar");

            if (!create) return;
        }

        PhotoCatalog catalog = FindCatalog();
        PhotoJournalSlot slotPrefab = BuildSlotPrefab();

        // ---------- Raiz y Canvas ----------
        GameObject root = new GameObject("PhotoSystem");
        AlbumManager album = root.AddComponent<AlbumManager>();

        GameObject canvasGO = NewUI("PhotoCanvas", root.transform);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        PhotoNotificationUI notif = canvasGO.AddComponent<PhotoNotificationUI>();
        PhotoJournalUI journal = canvasGO.AddComponent<PhotoJournalUI>();

        // ---------- Icono de aviso ----------
        GameObject prompt = NewUI("PhotoPromptIcon", canvasGO.transform);
        Anchor(Rect(prompt), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-40f, 0f), new Vector2(120f, 120f));
        AddImage(prompt, new Color(1f, 0.85f, 0.2f, 1f));
        AddLabelChild(prompt.transform, "FOTO", 30f, TextAlignmentOptions.Center, Color.black);
        prompt.SetActive(false);

        // ---------- Panel que sale al tomar la foto ----------
        GameObject reveal = NewUI("RevealPanel", canvasGO.transform);
        Anchor(Rect(reveal), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1100f, 520f));
        AddImage(reveal, PanelColor);

        GameObject revealPhoto = NewUI("PhotoImage", reveal.transform);
        Anchor(Rect(revealPhoto), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(40f, 0f), new Vector2(420f, 420f));
        Image revealImg = AddImage(revealPhoto, Color.white);
        revealImg.preserveAspect = true;

        GameObject revealTitle = NewUI("TitleText", reveal.transform);
        Inset(Rect(revealTitle), 500f, 410f, 40f, 40f);
        TextMeshProUGUI revealTitleText = AddText(revealTitle, "¡Foto capturada!", 44f, TextAlignmentOptions.Left, Color.white);

        GameObject revealDesc = NewUI("DescriptionText", reveal.transform);
        Inset(Rect(revealDesc), 500f, 100f, 40f, 120f);
        TextMeshProUGUI revealDescText = AddText(revealDesc, "Descripcion", 30f, TextAlignmentOptions.TopLeft, Color.white);

        GameObject revealClose = NewUI("CloseButton", reveal.transform);
        Anchor(Rect(revealClose), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-40f, 25f), new Vector2(220f, 70f));
        Button revealCloseBtn = AddButton(revealClose, ButtonColor, "Cerrar");
        UnityEventTools.AddVoidPersistentListener(revealCloseBtn.onClick, notif.Hide);

        reveal.SetActive(false);

        // ---------- Botones siempre visibles ----------
        GameObject journalBtnGO = NewUI("JournalButton", canvasGO.transform);
        Anchor(Rect(journalBtnGO), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-30f, -30f), new Vector2(220f, 80f));
        Button journalBtn = AddButton(journalBtnGO, ButtonColor, "Bitácora");
        UnityEventTools.AddVoidPersistentListener(journalBtn.onClick, journal.Toggle);

        GameObject photoBtnGO = NewUI("PhotoButton", canvasGO.transform);
        Anchor(Rect(photoBtnGO), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-40f, 380f), new Vector2(200f, 90f));
        Button photoBtn = AddButton(photoBtnGO, ButtonColor, "Foto");
        UnityEventTools.AddVoidPersistentListener(photoBtn.onClick, album.TakePhotoButton);

        // ---------- Bitacora ----------
        GameObject journalPanel = NewUI("JournalPanel", canvasGO.transform);
        Stretch(Rect(journalPanel));
        AddImage(journalPanel, PanelColor);

        GameObject jTitle = NewUI("TitleText", journalPanel.transform);
        Anchor(Rect(jTitle), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(600f, 80f));
        AddText(jTitle, "Bitácora", 60f, TextAlignmentOptions.Center, Color.white);

        GameObject jProgress = NewUI("ProgressText", journalPanel.transform);
        Anchor(Rect(jProgress), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-60f, -45f), new Vector2(260f, 60f));
        TextMeshProUGUI progressText = AddText(jProgress, "0 / 0", 40f, TextAlignmentOptions.Right, Color.white);

        GameObject jClose = NewUI("CloseButton", journalPanel.transform);
        Anchor(Rect(jClose), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(60f, -30f), new Vector2(220f, 70f));
        Button jCloseBtn = AddButton(jClose, ButtonColor, "Cerrar");
        UnityEventTools.AddVoidPersistentListener(jCloseBtn.onClick, journal.Close);

        // Scroll View
        GameObject scroll = NewUI("Scroll View", journalPanel.transform);
        RectTransform scrollRt = Rect(scroll);
        scrollRt.anchorMin = new Vector2(0.05f, 0.08f);
        scrollRt.anchorMax = new Vector2(0.95f, 0.82f);
        scrollRt.offsetMin = Vector2.zero;
        scrollRt.offsetMax = Vector2.zero;

        ScrollRect scrollRect = scroll.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.scrollSensitivity = 30f;

        GameObject viewport = NewUI("Viewport", scroll.transform);
        Stretch(Rect(viewport));
        AddImage(viewport, new Color(0f, 0f, 0f, 0.01f)); // permite arrastrar el scroll
        viewport.AddComponent<RectMask2D>();

        GameObject content = NewUI("Content", viewport.transform);
        RectTransform contentRt = Rect(content);
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.anchoredPosition = Vector2.zero;
        contentRt.sizeDelta = Vector2.zero;

        GridLayoutGroup grid = content.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(280f, 320f);
        grid.spacing = new Vector2(20f, 20f);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;
        grid.childAlignment = TextAnchor.UpperCenter;
        grid.padding = new RectOffset(20, 20, 20, 20);

        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = Rect(viewport);
        scrollRect.content = contentRt;

        // Detalle (encima de todo lo demas de la bitacora)
        GameObject detail = NewUI("DetailPanel", journalPanel.transform);
        Anchor(Rect(detail), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1000f, 700f));
        AddImage(detail, new Color(0.10f, 0.13f, 0.10f, 0.99f));

        GameObject detailPhoto = NewUI("DetailImage", detail.transform);
        Anchor(Rect(detailPhoto), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -60f), new Vector2(420f, 420f));
        Image detailImg = AddImage(detailPhoto, Color.white);
        detailImg.preserveAspect = true;

        GameObject detailTitle = NewUI("DetailTitle", detail.transform);
        Inset(Rect(detailTitle), 500f, 580f, 40f, 50f);
        TextMeshProUGUI detailTitleText = AddText(detailTitle, "Nombre", 46f, TextAlignmentOptions.Left, Color.white);

        GameObject detailCat = NewUI("DetailCategory", detail.transform);
        Inset(Rect(detailCat), 500f, 520f, 40f, 130f);
        TextMeshProUGUI detailCatText = AddText(detailCat, "Categoria", 30f, TextAlignmentOptions.Left, new Color(0.7f, 0.9f, 0.7f, 1f));

        GameObject detailDesc = NewUI("DetailDescription", detail.transform);
        Inset(Rect(detailDesc), 500f, 110f, 40f, 200f);
        TextMeshProUGUI detailDescText = AddText(detailDesc, "Descripcion", 30f, TextAlignmentOptions.TopLeft, Color.white);

        GameObject detailClose = NewUI("CloseDetailButton", detail.transform);
        Anchor(Rect(detailClose), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-40f, 30f), new Vector2(220f, 70f));
        Button detailCloseBtn = AddButton(detailClose, ButtonColor, "Volver");
        UnityEventTools.AddVoidPersistentListener(detailCloseBtn.onClick, journal.CloseDetail);

        detail.SetActive(false);
        journalPanel.SetActive(false);

        // ---------- Conectar los campos de los scripts ----------
        album.notificationUI = notif;
        album.photoPromptIcon = prompt;

        notif.notificationPanel = reveal;
        notif.notificationText = revealTitleText;
        notif.photoImage = revealImg;
        notif.descriptionText = revealDescText;

        journal.catalog = catalog;
        journal.journalPanel = journalPanel;
        journal.slotContainer = contentRt;
        journal.slotPrefab = slotPrefab;
        journal.progressText = progressText;
        journal.detailPanel = detail;
        journal.detailImage = detailImg;
        journal.detailTitle = detailTitleText;
        journal.detailCategory = detailCatText;
        journal.detailDescription = detailDescText;

        // ---------- Guardar como prefab ----------
        EnsurePrefabsFolder();
        PrefabUtility.SaveAsPrefabAssetAndConnect(
            root, "Assets/Prefabs/PhotoSystem.prefab", InteractionMode.UserAction);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = root;

        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            Debug.LogWarning("[PhotoSystem] Esta escena no tiene EventSystem: sin el, los botones no responden. Crealo con clic derecho en la Hierarchy -> UI -> Event System.");
        }

        if (catalog == null)
        {
            Debug.LogWarning("[PhotoSystem] No encontre ningun PhotoCatalog. Crealo (Create -> Pulso Ecologico -> Photo Catalog), y arrastralo al campo 'Catalog' del PhotoCanvas > PhotoJournalUI en el prefab.");
        }

        Debug.Log("[PhotoSystem] Listo. Prefab creado en Assets/Prefabs/PhotoSystem.prefab y ya esta en esta escena.");
    }

    // ---------- Casilla de la bitacora (prefab) ----------
    private static PhotoJournalSlot BuildSlotPrefab()
    {
        EnsurePrefabsFolder();

        GameObject slot = new GameObject("PhotoSlot", typeof(RectTransform));
        Rect(slot).sizeDelta = new Vector2(280f, 320f);

        Image bg = AddImage(slot, SlotColor);
        Button button = slot.AddComponent<Button>();
        button.targetGraphic = bg;

        GameObject photo = NewUI("Photo", slot.transform);
        Anchor(Rect(photo), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -15f), new Vector2(240f, 240f));
        Image photoImg = AddImage(photo, Color.white);
        photoImg.preserveAspect = true;
        photoImg.raycastTarget = false;

        GameObject nameGO = NewUI("Name", slot.transform);
        RectTransform nameRt = Rect(nameGO);
        nameRt.anchorMin = new Vector2(0f, 0f);
        nameRt.anchorMax = new Vector2(1f, 0f);
        nameRt.pivot = new Vector2(0.5f, 0f);
        nameRt.anchoredPosition = new Vector2(0f, 10f);
        nameRt.sizeDelta = new Vector2(-20f, 50f);
        TextMeshProUGUI nameText = AddText(nameGO, "???", 28f, TextAlignmentOptions.Center, Color.white);
        nameText.raycastTarget = false;

        PhotoJournalSlot slotScript = slot.AddComponent<PhotoJournalSlot>();
        slotScript.photoImage = photoImg;
        slotScript.nameText = nameText;
        slotScript.button = button;

        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(slot, "Assets/Prefabs/PhotoSlot.prefab");
        Object.DestroyImmediate(slot);

        return savedPrefab.GetComponent<PhotoJournalSlot>();
    }

    private static PhotoCatalog FindCatalog()
    {
        string[] guids = AssetDatabase.FindAssets("t:PhotoCatalog");
        if (guids.Length == 0) return null;

        return AssetDatabase.LoadAssetAtPath<PhotoCatalog>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }

    private static void EnsurePrefabsFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
    }

    // ---------- Ayudas para armar UI ----------
    private static GameObject NewUI(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static RectTransform Rect(GameObject go)
    {
        return go.GetComponent<RectTransform>();
    }

    private static void Stretch(RectTransform r)
    {
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
    }

    // Ocupa todo el padre, dejando margenes por lado.
    private static void Inset(RectTransform r, float left, float bottom, float right, float top)
    {
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = new Vector2(left, bottom);
        r.offsetMax = new Vector2(-right, -top);
    }

    private static void Anchor(RectTransform r, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        r.anchorMin = anchor;
        r.anchorMax = anchor;
        r.pivot = pivot;
        r.anchoredPosition = position;
        r.sizeDelta = size;
    }

    private static Image AddImage(GameObject go, Color color)
    {
        Image img = go.AddComponent<Image>();
        img.color = color;
        return img;
    }

    private static TextMeshProUGUI AddText(GameObject go, string text, float size, TextAlignmentOptions align, Color color)
    {
        TextMeshProUGUI t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.alignment = align;
        t.color = color;
        return t;
    }

    private static void AddLabelChild(Transform parent, string text, float size, TextAlignmentOptions align, Color color)
    {
        GameObject label = NewUI("Label", parent);
        Stretch(Rect(label));
        TextMeshProUGUI t = AddText(label, text, size, align, color);
        t.raycastTarget = false;
    }

    private static Button AddButton(GameObject go, Color bg, string label)
    {
        Image img = AddImage(go, bg);
        Button b = go.AddComponent<Button>();
        b.targetGraphic = img;
        AddLabelChild(go.transform, label, 30f, TextAlignmentOptions.Center, Color.white);
        return b;
    }
}
#endif