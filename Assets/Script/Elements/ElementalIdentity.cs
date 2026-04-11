using UnityEngine;

public class ElementalIdentity : MonoBehaviour
{
    [SerializeField] private ElementalType elementType;

    public ElementalType Type => elementType;
}
