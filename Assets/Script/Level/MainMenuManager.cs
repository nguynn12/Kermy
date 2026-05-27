using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject aboutCanvas;
    public GameObject optionsCanvas;
    public GameObject controlsCanvas;

    [Header("Scene Selection (kéo thả scene vào)")]
#if UNITY_EDITOR
    public SceneAsset targetScene;
#endif
    [Tooltip("Tên scene sẽ load (tự động lấy từ targetScene)")]
    public string sceneName = "Level01";

    private void Start()
    {
        if (aboutCanvas != null) aboutCanvas.SetActive(false);
        if (optionsCanvas != null) optionsCanvas.SetActive(false);
        if (controlsCanvas != null) controlsCanvas.SetActive(false);

#if UNITY_EDITOR
        if (targetScene != null)
            sceneName = targetScene.name;
#endif
    }

    public void PlayGame()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Chưa gán scene cần load! Hãy kéo scene vào ô targetScene hoặc gõ tên scene trong sceneName.");
            return;
        }

        string sceneToLoad = IsSceneInBuildSettings(sceneName)
            ? sceneName
            : FindFirstLevelSceneInBuildSettings();

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Chưa có scene Level nào trong Build Settings.");
            return;
        }

        Debug.Log($"Đang load scene: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OpenAbout()
    {
        if (aboutCanvas != null) aboutCanvas.SetActive(true);
    }

    public void CloseAbout()
    {
        if (aboutCanvas != null) aboutCanvas.SetActive(false);
    }

    public void OpenOptions()
    {
        EnsureOptionsUI();
        if (aboutCanvas != null) aboutCanvas.SetActive(false);
        if (controlsCanvas != null) controlsCanvas.SetActive(false);
        if (optionsCanvas != null) optionsCanvas.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsCanvas != null) optionsCanvas.SetActive(false);
    }

    public void OpenControls()
    {
        EnsureOptionsUI();
        if (optionsCanvas != null) optionsCanvas.SetActive(false);
        if (controlsCanvas != null) controlsCanvas.SetActive(true);
    }

    public void CloseControls()
    {
        if (controlsCanvas != null) controlsCanvas.SetActive(false);
        if (optionsCanvas != null) optionsCanvas.SetActive(true);
    }

    public void ResetControls()
    {
        KeybindingManager.ResetOverrides(KeybindingManager.Actions);
        foreach (KeybindingButton bindingButton in FindObjectsByType<KeybindingButton>(FindObjectsInactive.Include))
        {
            bindingButton.RefreshLabel();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Đang thoát game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool IsSceneInBuildSettings(string targetName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string buildSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (buildSceneName == targetName)
                return true;
        }

        return false;
    }

    private string FindFirstLevelSceneInBuildSettings()
    {
        string bestScene = null;
        int bestLevel = int.MaxValue;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string buildSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (!TryParseLevelNumber(buildSceneName, out int levelNumber))
                continue;

            if (levelNumber < bestLevel)
            {
                bestLevel = levelNumber;
                bestScene = buildSceneName;
            }
        }

        return bestScene;
    }

    private bool TryParseLevelNumber(string levelSceneName, out int levelNumber)
    {
        levelNumber = 0;
        if (string.IsNullOrEmpty(levelSceneName) || !levelSceneName.StartsWith("Level", System.StringComparison.OrdinalIgnoreCase))
            return false;

        string digits = levelSceneName.Substring("Level".Length);
        return int.TryParse(digits, out levelNumber);
    }

    private void EnsureOptionsUI()
    {
        Canvas canvas = aboutCanvas != null ? aboutCanvas.GetComponentInParent<Canvas>() : FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
        }

        EnsureEventSystem();

        if (optionsCanvas == null)
        {
            optionsCanvas = CreateOptionsPanel(canvas.transform);
            optionsCanvas.SetActive(false);
        }

        if (controlsCanvas == null)
        {
            controlsCanvas = CreateControlsPanel(canvas.transform);
            controlsCanvas.SetActive(false);
        }
    }

    private GameObject CreateOptionsPanel(Transform parent)
    {
        GameObject root = CreatePanelRoot("OptionsPanel_Runtime", parent);
        RectTransform panel = CreateBox("Panel", root.transform, new Vector2(520f, 500f), new Color(0.08f, 0.08f, 0.1f, 0.94f));

        CreateLabel("Title", panel, "OPTIONS", 40, new Vector2(0f, 175f), new Vector2(420f, 60f));

        Toggle fullscreenToggle = CreateToggle("Fullscreen Toggle", panel, "Fullscreen", new Vector2(0f, 90f));
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(value => Screen.fullScreen = value);

        CreateLabel("Volume Label", panel, "Volume", 24, new Vector2(0f, 25f), new Vector2(360f, 36f));
        Slider volumeSlider = CreateSlider("Volume Slider", panel, new Vector2(0f, -25f));
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(value => AudioListener.volume = value);

        CreateButton("Controls Button", panel, "KEYBINDS", new Vector2(0f, -105f), new Vector2(280f, 62f), OpenControls);
        CreateButton("Options Back Button", panel, "BACK", new Vector2(0f, -185f), new Vector2(220f, 56f), CloseOptions);
        return root;
    }

    private GameObject CreateControlsPanel(Transform parent)
    {
        GameObject root = CreatePanelRoot("ControlsPanel_Runtime", parent);
        RectTransform panel = CreateBox("Panel", root.transform, new Vector2(940f, 760f), new Color(0.07f, 0.07f, 0.09f, 0.96f));

        CreateLabel("Title", panel, "KEYBINDS", 38, new Vector2(0f, 320f), new Vector2(520f, 54f));
        CreateBindingColumn(panel, "IGNUS", "Player1_Map", -235f);
        CreateBindingColumn(panel, "AQUA", "Player2_Map", 235f);

        CreateButton("Reset Controls Button", panel, "RESET", new Vector2(-130f, -330f), new Vector2(220f, 54f), ResetControls);
        CreateButton("Controls Back Button", panel, "BACK", new Vector2(130f, -330f), new Vector2(220f, 54f), CloseControls);
        return root;
    }

    private void CreateBindingColumn(RectTransform parent, string title, string mapName, float x)
    {
        CreateLabel(title + " Title", parent, title, 30, new Vector2(x, 260f), new Vector2(330f, 42f));
        CreateBindingRow(parent, "Left", mapName, "Move", "left", null, x, 200f);
        CreateBindingRow(parent, "Right", mapName, "Move", "right", null, x, 142f);
        CreateBindingRow(parent, "Up", mapName, "Move", "up", "Jump", x, 84f);
        CreateBindingRow(parent, "Down", mapName, "Move", "down", null, x, 26f);
        CreateBindingRow(parent, "Jump", mapName, "Jump", null, null, x, -32f);
        CreateBindingRow(parent, "Action", mapName, "Action", null, null, x, -90f);
    }

    private void CreateBindingRow(RectTransform parent, string label, string mapName, string actionName, string bindingName, string linkedAction, float x, float y)
    {
        CreateLabel(label + " Label " + mapName, parent, label, 22, new Vector2(x - 85f, y), new Vector2(140f, 38f));
        Button button = CreateButton(label + " Button " + mapName, parent, "-", new Vector2(x + 80f, y), new Vector2(150f, 40f), null);
        KeybindingButton bindingButton = button.gameObject.AddComponent<KeybindingButton>();
        bindingButton.Configure(mapName, actionName, bindingName, linkedAction);
    }

    private static GameObject CreatePanelRoot(string name, Transform parent)
    {
        GameObject root = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        root.transform.SetParent(parent, false);
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        root.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.72f);
        return root;
    }

    private static RectTransform CreateBox(string name, Transform parent, Vector2 size, Color color)
    {
        GameObject box = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        box.transform.SetParent(parent, false);
        RectTransform rect = box.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        box.GetComponent<Image>().color = color;
        return rect;
    }

    private static TextMeshProUGUI CreateLabel(string name, Transform parent, string text, float fontSize, Vector2 position, Vector2 size)
    {
        GameObject label = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        label.transform.SetParent(parent, false);
        RectTransform rect = label.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        TextMeshProUGUI textComponent = label.GetComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;
        return textComponent;
    }

    private static Button CreateButton(string name, Transform parent, string text, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        buttonObject.GetComponent<Image>().color = new Color(0.92f, 0.92f, 0.92f, 1f);
        Button button = buttonObject.GetComponent<Button>();
        if (onClick != null) button.onClick.AddListener(onClick);

        TextMeshProUGUI label = CreateLabel("Text", buttonObject.transform, text, 22, Vector2.zero, size);
        label.color = Color.black;
        label.fontStyle = FontStyles.Bold;
        return button;
    }

    private static Toggle CreateToggle(string name, Transform parent, string text, Vector2 position)
    {
        GameObject toggleObject = new GameObject(name, typeof(RectTransform), typeof(Toggle));
        toggleObject.transform.SetParent(parent, false);
        RectTransform rect = toggleObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(320f, 42f);

        Image background = CreateSmallImage("Background", toggleObject.transform, new Vector2(-125f, 0f), new Vector2(32f, 32f), Color.white);
        Image checkmark = CreateSmallImage("Checkmark", background.transform, Vector2.zero, new Vector2(22f, 22f), new Color(0.1f, 0.7f, 0.25f));
        Toggle toggle = toggleObject.GetComponent<Toggle>();
        toggle.targetGraphic = background;
        toggle.graphic = checkmark;

        TextMeshProUGUI label = CreateLabel("Label", toggleObject.transform, text, 24, new Vector2(25f, 0f), new Vector2(240f, 42f));
        label.alignment = TextAlignmentOptions.Left;
        return toggle;
    }

    private static Slider CreateSlider(string name, Transform parent, Vector2 position)
    {
        GameObject sliderObject = new GameObject(name, typeof(RectTransform), typeof(Slider));
        sliderObject.transform.SetParent(parent, false);
        RectTransform rect = sliderObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(330f, 34f);

        RectTransform background = CreateBox("Background", sliderObject.transform, new Vector2(330f, 12f), new Color(0.35f, 0.35f, 0.38f, 1f));
        RectTransform fillArea = CreateBox("Fill Area", sliderObject.transform, new Vector2(300f, 12f), Color.clear);
        RectTransform fill = CreateBox("Fill", fillArea, new Vector2(300f, 12f), new Color(0.25f, 0.65f, 1f, 1f));
        RectTransform handle = CreateBox("Handle", sliderObject.transform, new Vector2(24f, 24f), Color.white);

        Slider slider = sliderObject.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.targetGraphic = handle.GetComponent<Image>();
        slider.fillRect = fill;
        slider.handleRect = handle;
        background.SetAsFirstSibling();
        return slider;
    }

    private static Image CreateSmallImage(string name, Transform parent, Vector2 position, Vector2 size, Color color)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }
}
