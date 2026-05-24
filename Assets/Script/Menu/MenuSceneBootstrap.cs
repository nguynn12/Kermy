using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[ExecuteAlways]
public class MenuSceneBootstrap : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "Level01";
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject controlsPanel;

    private void Awake()
    {
        EnsureCamera();
        EnsureEventSystem();
        NormalizeMenuLayout();

        if (mainMenu == null)
        {
            mainMenu = GetComponent<MainMenu>();
        }

        if (optionsPanel == null)
        {
            optionsPanel = FindSceneObject("Options Panel");
        }

        if (controlsPanel == null)
        {
            controlsPanel = FindSceneObject("Controls Panel");
        }

        if (mainMenu != null)
        {
            mainMenu.Configure(gameplaySceneName, optionsPanel);
            AddButtonListener("Play Button", mainMenu.PlayGame);
            AddButtonListener("Options Button", mainMenu.ShowOptions);
            AddButtonListener("Quit Button", mainMenu.QuitGame);
            AddButtonListener("Options Back Button", mainMenu.HideOptions);
        }

        AddButtonListener("Controls Button", ShowControlsPanel);
        AddButtonListener("Reset Controls Button", ResetControls);
        AddButtonListener("Controls Back Button", HideControlsPanel);
        AddToggleListener("Fullscreen Toggle", SetFullscreen);
        AddSliderListener("Volume Slider", SetVolume);

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }

    private void OnValidate()
    {
        NormalizeMenuLayout();
    }

    public void ShowControlsPanel()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
    }

    public void HideControlsPanel()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }

        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    public void ResetControls()
    {
        KeybindingManager.ResetOverrides(KeybindingManager.Actions);
        RefreshBindingButtons();
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    private void NormalizeMenuLayout()
    {
        StretchToParent("Scrolling Background");
        StretchToParent("Menu Contrast Overlay");
        StretchToParent("Menu Layout");

        SetRect("Title Text", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -150f), new Vector2(760f, 120f));
        SetRect("Subtitle Text", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -235f), new Vector2(720f, 46f));
        SetRect("Buttons", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -110f), new Vector2(300f, 230f));
        SetRect("Options Panel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(430f, 455f));
        SetRect("Controls Panel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720f, 860f));
    }

    private void RefreshBindingButtons()
    {
        foreach (KeybindingButton bindingButton in FindObjectsByType<KeybindingButton>(FindObjectsInactive.Include))
        {
            bindingButton.RefreshLabel();
        }
    }

    private static void EnsureCamera()
    {
        if (Camera.main != null)
        {
            return;
        }

        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
        camera.orthographic = true;
        cameraObject.tag = "MainCamera";
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

    private static GameObject FindSceneObject(string objectName)
    {
        foreach (Transform transform in FindObjectsByType<Transform>(FindObjectsInactive.Include))
        {
            if (transform.name == objectName)
            {
                return transform.gameObject;
            }
        }

        return null;
    }

    private static void StretchToParent(string objectName)
    {
        RectTransform rect = FindRect(objectName);
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    private static void SetRect(string objectName, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        RectTransform rect = FindRect(objectName);
        if (rect == null)
        {
            return;
        }

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        rect.localScale = Vector3.one;
    }

    private static RectTransform FindRect(string objectName)
    {
        GameObject target = FindSceneObject(objectName);
        return target != null ? target.GetComponent<RectTransform>() : null;
    }

    private static void AddButtonListener(string objectName, UnityEngine.Events.UnityAction action)
    {
        GameObject target = FindSceneObject(objectName);
        Button button = target != null ? target.GetComponent<Button>() : null;
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    private static void AddToggleListener(string objectName, UnityEngine.Events.UnityAction<bool> action)
    {
        GameObject target = FindSceneObject(objectName);
        Toggle toggle = target != null ? target.GetComponent<Toggle>() : null;
        if (toggle == null)
        {
            return;
        }

        toggle.onValueChanged.RemoveListener(action);
        toggle.onValueChanged.AddListener(action);
    }

    private static void AddSliderListener(string objectName, UnityEngine.Events.UnityAction<float> action)
    {
        GameObject target = FindSceneObject(objectName);
        Slider slider = target != null ? target.GetComponent<Slider>() : null;
        if (slider == null)
        {
            return;
        }

        slider.onValueChanged.RemoveListener(action);
        slider.onValueChanged.AddListener(action);
    }
}
