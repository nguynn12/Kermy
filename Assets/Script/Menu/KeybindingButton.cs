using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class KeybindingButton : MonoBehaviour
{
    [SerializeField] private string mapName;
    [SerializeField] private string actionName;
    [SerializeField] private string bindingName;
    [SerializeField] private string linkedButtonAction;

    private Button button;
    private InputActionRebindingExtensions.RebindingOperation activeRebindOperation;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartRebind);
        RefreshLabel();
    }

    private void OnEnable()
    {
        RefreshLabel();
    }

    private void OnDisable()
    {
        CancelActiveRebind();
    }

    public void Configure(string actionMap, string inputAction, string bindingPart, string linkedAction = null)
    {
        mapName = actionMap;
        actionName = inputAction;
        bindingName = bindingPart;
        linkedButtonAction = linkedAction;
    }

    public void RefreshLabel()
    {
        SetButtonText(GetBindingDisplayName());
    }

    private void StartRebind()
    {
        CancelActiveRebind();

        InputAction action = GetAction();
        int bindingIndex = GetBindingIndex(action);
        if (action == null || bindingIndex < 0)
        {
            return;
        }

        SetButtonText("Press your key");
        action.Disable();

        activeRebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .WithControlsExcluding("<Mouse>")
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .WithControlsExcluding("<Mouse>/scroll")
            .OnCancel(operation => FinishRebind(action, operation, false))
            .OnComplete(operation => FinishRebind(action, operation, true))
            .Start();
    }

    private void FinishRebind(InputAction action, InputActionRebindingExtensions.RebindingOperation operation, bool completed)
    {
        operation.Dispose();
        activeRebindOperation = null;
        action.Enable();

        if (completed)
        {
            ApplyLinkedBinding();
            KeybindingManager.SaveOverrides(KeybindingManager.Actions);
        }

        RefreshLabel();
    }

    private void CancelActiveRebind()
    {
        if (activeRebindOperation == null)
        {
            return;
        }

        activeRebindOperation.Cancel();
    }

    private string GetBindingDisplayName()
    {
        InputAction action = GetAction();
        int bindingIndex = GetBindingIndex(action);
        if (action == null || bindingIndex < 0)
        {
            return "-";
        }

        return action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
    }

    private InputAction GetAction()
    {
        return KeybindingManager.Actions?.FindActionMap(mapName)?.FindAction(actionName);
    }

    private int GetBindingIndex(InputAction action)
    {
        if (action == null)
        {
            return -1;
        }

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];
            if (!string.IsNullOrEmpty(bindingName))
            {
                if (binding.isPartOfComposite && binding.name == bindingName)
                {
                    return i;
                }
            }
            else if (!binding.isComposite && !binding.isPartOfComposite)
            {
                return i;
            }
        }

        return -1;
    }

    private void ApplyLinkedBinding()
    {
        if (string.IsNullOrEmpty(linkedButtonAction))
        {
            return;
        }

        InputAction sourceAction = GetAction();
        int sourceIndex = GetBindingIndex(sourceAction);
        InputAction linkedAction = KeybindingManager.Actions?.FindActionMap(mapName)?.FindAction(linkedButtonAction);
        int linkedIndex = GetFirstSimpleBindingIndex(linkedAction);

        if (sourceAction == null || linkedAction == null || sourceIndex < 0 || linkedIndex < 0)
        {
            return;
        }

        string overridePath = sourceAction.bindings[sourceIndex].effectivePath;
        linkedAction.ApplyBindingOverride(linkedIndex, overridePath);
    }

    private void SetButtonText(string value)
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        Text text = button.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = value;
        }
    }

    private static int GetFirstSimpleBindingIndex(InputAction action)
    {
        if (action == null)
        {
            return -1;
        }

        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];
            if (!binding.isComposite && !binding.isPartOfComposite)
            {
                return i;
            }
        }

        return -1;
    }
}
