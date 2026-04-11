using UnityEngine;
using UnityEngine.Events;

public class BoolInverter : MonoBehaviour
{
    [SerializeField] private UnityEvent<bool> onOutput;

    public void Input(bool value)
    {
        onOutput?.Invoke(!value);
    }
}
