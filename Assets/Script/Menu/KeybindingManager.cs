using UnityEngine;
using UnityEngine.InputSystem;

public static class KeybindingManager
{
    private const string BindingOverridesKey = "Kermy.BindingOverrides";
    private const string InputActionsResourcePath = "Input/PlayerControls";

    private static InputActionAsset cachedActions;

    public static InputActionAsset Actions
    {
        get
        {
            if (cachedActions == null)
            {
                cachedActions = Resources.Load<InputActionAsset>(InputActionsResourcePath);
                ApplySavedOverrides(cachedActions);
            }

            return cachedActions;
        }
    }

    public static void ApplySavedOverrides(InputActionAsset actions)
    {
        if (actions == null || !PlayerPrefs.HasKey(BindingOverridesKey))
        {
            return;
        }

        string overrides = PlayerPrefs.GetString(BindingOverridesKey);
        if (!string.IsNullOrEmpty(overrides))
        {
            actions.LoadBindingOverridesFromJson(overrides);
        }
    }

    public static void SaveOverrides(InputActionAsset actions)
    {
        if (actions == null)
        {
            return;
        }

        PlayerPrefs.SetString(BindingOverridesKey, actions.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }

    public static void ResetOverrides(InputActionAsset actions)
    {
        if (actions != null)
        {
            actions.RemoveAllBindingOverrides();
        }

        PlayerPrefs.DeleteKey(BindingOverridesKey);
        PlayerPrefs.Save();
    }
}
