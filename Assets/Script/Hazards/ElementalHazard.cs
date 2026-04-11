using UnityEngine;

public class ElementalHazard : MonoBehaviour
{
    [SerializeField] private ElementalType hazardType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ElementalIdentity identity = other.GetComponent<ElementalIdentity>();
        if (identity == null)
        {
            return;
        }

        if (identity.Type == hazardType)
        {
            return;
        }

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Kill();
        }
    }
}
