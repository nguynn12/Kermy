using UnityEngine;

public class ElementalHazard : MonoBehaviour
{
    [SerializeField] private ElementalType hazardType;

    public ElementalType HazardType => hazardType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        BlockHazardInteraction blockInteraction = other.GetComponent<BlockHazardInteraction>();
        if (blockInteraction != null)
        {
            blockInteraction.OnTouchedHazard(this);
            return;
        }

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
