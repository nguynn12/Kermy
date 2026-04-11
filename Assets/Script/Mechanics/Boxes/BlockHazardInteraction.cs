using UnityEngine;

public class BlockHazardInteraction : MonoBehaviour
{
    [SerializeField] private PushableBox pushableBox;

    [Header("Obsidian Conversion")]
    [SerializeField] private int obsidianLayer = 0;

    private void Reset()
    {
        pushableBox = GetComponent<PushableBox>();
        obsidianLayer = LayerMask.NameToLayer("Ground");
    }

    public void OnTouchedHazard(ElementalHazard hazard)
    {
        if (hazard == null || pushableBox == null)
        {
            return;
        }

        if (!ShouldConvertToObsidian(hazard.HazardType, pushableBox.Type))
        {
            return;
        }

        ConvertHazardToObsidian(hazard.gameObject);
        Destroy(gameObject);
    }

    private static bool ShouldConvertToObsidian(ElementalType hazardType, PushableBox.BoxType boxType)
    {
        if (boxType == PushableBox.BoxType.Ice && hazardType == ElementalType.Fire)
        {
            return true;
        }

        if (boxType == PushableBox.BoxType.Lava && hazardType == ElementalType.Water)
        {
            return true;
        }

        return false;
    }

    private void ConvertHazardToObsidian(GameObject hazardObject)
    {
        if (hazardObject == null)
        {
            return;
        }

        ElementalHazard hazard = hazardObject.GetComponent<ElementalHazard>();
        if (hazard != null)
        {
            hazard.enabled = false;
        }

        if (obsidianLayer >= 0)
        {
            hazardObject.layer = obsidianLayer;
        }

        Collider2D col = hazardObject.GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = false;
        }

        Rigidbody2D rb = hazardObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
}
