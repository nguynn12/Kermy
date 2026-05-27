using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class KeybindingManager
{
    private const string BindingOverridesKey = "Kermy.BindingOverrides";
    private const string InputActionsResourcePath = "Input/PlayerControls";
    private const string Player1Map = "Player1_Map";
    private const string Player2Map = "Player2_Map";
    private const string MoveAction = "Move";
    private const string JumpAction = "Jump";
    private const string ActionButton = "Action";

    private static InputActionAsset cachedActions;
    private static readonly Dictionary<string, KeyCode> SpecialKeys = new Dictionary<string, KeyCode>(StringComparer.OrdinalIgnoreCase)
    {
        { "space", KeyCode.Space },
        { "enter", KeyCode.Return },
        { "return", KeyCode.Return },
        { "tab", KeyCode.Tab },
        { "backspace", KeyCode.Backspace },
        { "delete", KeyCode.Delete },
        { "escape", KeyCode.Escape },
        { "upArrow", KeyCode.UpArrow },
        { "downArrow", KeyCode.DownArrow },
        { "leftArrow", KeyCode.LeftArrow },
        { "rightArrow", KeyCode.RightArrow },
        { "leftShift", KeyCode.LeftShift },
        { "rightShift", KeyCode.RightShift },
        { "leftCtrl", KeyCode.LeftControl },
        { "rightCtrl", KeyCode.RightControl },
        { "leftAlt", KeyCode.LeftAlt },
        { "rightAlt", KeyCode.RightAlt },
        { "minus", KeyCode.Minus },
        { "equals", KeyCode.Equals },
        { "leftBracket", KeyCode.LeftBracket },
        { "rightBracket", KeyCode.RightBracket },
        { "semicolon", KeyCode.Semicolon },
        { "quote", KeyCode.Quote },
        { "comma", KeyCode.Comma },
        { "period", KeyCode.Period },
        { "slash", KeyCode.Slash },
        { "backslash", KeyCode.Backslash },
        { "backquote", KeyCode.BackQuote },
        { "numpadEnter", KeyCode.KeypadEnter },
        { "numpadDivide", KeyCode.KeypadDivide },
        { "numpadMultiply", KeyCode.KeypadMultiply },
        { "numpadMinus", KeyCode.KeypadMinus },
        { "numpadPlus", KeyCode.KeypadPlus },
        { "numpadPeriod", KeyCode.KeypadPeriod }
    };

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

    public static Vector2 GetMoveInputForTag(string tagName)
    {
        return GetMoveInputForMap(GetMapNameForTag(tagName));
    }

    public static Vector2 GetMoveInputForMap(string mapName)
    {
        if (string.IsNullOrEmpty(mapName))
        {
            return Vector2.zero;
        }

        Vector2 move = Vector2.zero;
        if (GetKey(mapName, MoveAction, "left")) move.x -= 1f;
        if (GetKey(mapName, MoveAction, "right")) move.x += 1f;
        if (GetKey(mapName, MoveAction, "down")) move.y -= 1f;
        if (GetKey(mapName, MoveAction, "up")) move.y += 1f;
        return Vector2.ClampMagnitude(move, 1f);
    }

    public static bool GetJumpDownForTag(string tagName)
    {
        return GetKeyDown(GetMapNameForTag(tagName), JumpAction);
    }

    public static bool GetActionDownForTag(string tagName)
    {
        return GetKeyDown(GetMapNameForTag(tagName), ActionButton);
    }

    public static bool GetActionHeldForTag(string tagName)
    {
        return GetKey(GetMapNameForTag(tagName), ActionButton);
    }

    public static bool GetActionUpForTag(string tagName)
    {
        return GetKeyUp(GetMapNameForTag(tagName), ActionButton);
    }

    public static string GetActionDisplayNameForTag(string tagName)
    {
        return GetBindingDisplayName(GetMapNameForTag(tagName), ActionButton);
    }

    public static bool IsActionControlForTag(string tagName, string controlPath)
    {
        string expectedPath = GetBindingPath(GetMapNameForTag(tagName), ActionButton);
        return !string.IsNullOrEmpty(expectedPath)
            && string.Equals(NormalizeKeyboardPath(expectedPath), NormalizeKeyboardPath(controlPath), StringComparison.OrdinalIgnoreCase);
    }

    public static string GetMapNameForTag(string tagName)
    {
        string normalized = NormalizePlayerTag(tagName);
        if (normalized == "Player1") return Player1Map;
        if (normalized == "Player2") return Player2Map;
        return string.Empty;
    }

    public static bool HasBindingConflict(InputAction changedAction, int changedBindingIndex, out string conflictDisplayName)
    {
        conflictDisplayName = string.Empty;
        if (changedAction == null || changedBindingIndex < 0 || changedBindingIndex >= changedAction.bindings.Count)
        {
            return false;
        }

        InputBinding changedBinding = changedAction.bindings[changedBindingIndex];
        string changedPath = changedBinding.effectivePath;
        if (string.IsNullOrEmpty(changedPath))
        {
            return false;
        }

        InputActionAsset asset = changedAction.actionMap != null ? changedAction.actionMap.asset : Actions;
        if (asset == null)
        {
            return false;
        }

        string changedNormalizedPath = NormalizeKeyboardPath(changedPath);
        foreach (InputActionMap map in asset.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    InputBinding binding = action.bindings[i];
                    if (binding.isComposite || string.IsNullOrEmpty(binding.effectivePath))
                    {
                        continue;
                    }

                    if (action == changedAction && i == changedBindingIndex)
                    {
                        continue;
                    }

                    if (!string.Equals(NormalizeKeyboardPath(binding.effectivePath), changedNormalizedPath, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (IsAllowedSharedBinding(changedAction, changedBinding, action, binding))
                    {
                        continue;
                    }

                    conflictDisplayName = FormatBindingOwner(action, binding);
                    return true;
                }
            }
        }

        return false;
    }

    private static bool GetKey(string mapName, string actionName, string bindingName = null)
    {
        KeyCode key = GetKeyCode(mapName, actionName, bindingName);
        return key != KeyCode.None && Input.GetKey(key);
    }

    private static bool GetKeyDown(string mapName, string actionName, string bindingName = null)
    {
        KeyCode key = GetKeyCode(mapName, actionName, bindingName);
        return key != KeyCode.None && Input.GetKeyDown(key);
    }

    private static bool GetKeyUp(string mapName, string actionName, string bindingName = null)
    {
        KeyCode key = GetKeyCode(mapName, actionName, bindingName);
        return key != KeyCode.None && Input.GetKeyUp(key);
    }

    private static KeyCode GetKeyCode(string mapName, string actionName, string bindingName = null)
    {
        return PathToKeyCode(GetBindingPath(mapName, actionName, bindingName));
    }

    private static string GetBindingPath(string mapName, string actionName, string bindingName = null)
    {
        if (string.IsNullOrEmpty(mapName) || string.IsNullOrEmpty(actionName))
        {
            return string.Empty;
        }

        InputAction action = Actions?.FindActionMap(mapName)?.FindAction(actionName);
        int bindingIndex = FindBindingIndex(action, bindingName);
        return bindingIndex >= 0 ? action.bindings[bindingIndex].effectivePath : string.Empty;
    }

    private static string GetBindingDisplayName(string mapName, string actionName, string bindingName = null)
    {
        if (string.IsNullOrEmpty(mapName) || string.IsNullOrEmpty(actionName))
        {
            return "-";
        }

        InputAction action = Actions?.FindActionMap(mapName)?.FindAction(actionName);
        int bindingIndex = FindBindingIndex(action, bindingName);
        if (action == null || bindingIndex < 0)
        {
            return "-";
        }

        return action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
    }

    private static int FindBindingIndex(InputAction action, string bindingName = null)
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

    private static KeyCode PathToKeyCode(string path)
    {
        string keyName = NormalizeKeyboardPath(path);
        if (string.IsNullOrEmpty(keyName))
        {
            return KeyCode.None;
        }

        KeyCode specialKey;
        if (SpecialKeys.TryGetValue(keyName, out specialKey))
        {
            return specialKey;
        }

        if (keyName.Length == 1)
        {
            char keyChar = keyName[0];
            if (keyChar >= 'a' && keyChar <= 'z')
            {
                return (KeyCode)Enum.Parse(typeof(KeyCode), char.ToUpperInvariant(keyChar).ToString());
            }

            if (keyChar >= '0' && keyChar <= '9')
            {
                return (KeyCode)((int)KeyCode.Alpha0 + (keyChar - '0'));
            }
        }

        if (keyName.StartsWith("digit", StringComparison.OrdinalIgnoreCase) && keyName.Length == 6)
        {
            char digit = keyName[5];
            if (digit >= '0' && digit <= '9')
            {
                return (KeyCode)((int)KeyCode.Alpha0 + (digit - '0'));
            }
        }

        if (keyName.StartsWith("numpad", StringComparison.OrdinalIgnoreCase) && keyName.Length == 7)
        {
            char digit = keyName[6];
            if (digit >= '0' && digit <= '9')
            {
                return (KeyCode)((int)KeyCode.Keypad0 + (digit - '0'));
            }
        }

        if (keyName.StartsWith("f", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(keyName.Substring(1), out int functionKey)
            && functionKey >= 1
            && functionKey <= 15)
        {
            return (KeyCode)((int)KeyCode.F1 + functionKey - 1);
        }

        return KeyCode.None;
    }

    private static string NormalizeKeyboardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        string normalized = path.Replace("<Keyboard>/", string.Empty)
                                .Replace("/Keyboard/", string.Empty)
                                .Replace("Keyboard/", string.Empty)
                                .Trim()
                                .TrimStart('/');
        return normalized;
    }

    private static string NormalizePlayerTag(string tagName)
    {
        return string.IsNullOrEmpty(tagName) ? string.Empty : tagName.Replace(" ", string.Empty);
    }

    private static bool IsAllowedSharedBinding(InputAction firstAction, InputBinding firstBinding, InputAction secondAction, InputBinding secondBinding)
    {
        if (firstAction == null || secondAction == null || firstAction.actionMap != secondAction.actionMap)
        {
            return false;
        }

        return IsJumpAndMoveUpPair(firstAction, firstBinding, secondAction, secondBinding)
            || IsJumpAndMoveUpPair(secondAction, secondBinding, firstAction, firstBinding);
    }

    private static bool IsJumpAndMoveUpPair(InputAction jumpAction, InputBinding jumpBinding, InputAction moveAction, InputBinding moveBinding)
    {
        return jumpAction.name == JumpAction
            && !jumpBinding.isComposite
            && !jumpBinding.isPartOfComposite
            && moveAction.name == MoveAction
            && moveBinding.isPartOfComposite
            && moveBinding.name == "up";
    }

    private static string FormatBindingOwner(InputAction action, InputBinding binding)
    {
        string mapName = action.actionMap != null ? action.actionMap.name.Replace("_Map", string.Empty) : "Player";
        string bindingName = string.IsNullOrEmpty(binding.name) ? action.name : action.name + " " + binding.name;
        return mapName + " " + bindingName;
    }
}
