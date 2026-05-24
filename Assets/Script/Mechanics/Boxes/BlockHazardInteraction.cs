using UnityEngine;
using UnityEngine.Tilemaps;

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

        if (hazard.GetComponent<Tilemap>() != null)
        {
            if (TryConvertHazardTilemap(hazard))
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning($"Could not convert hazard tilemap '{hazard.name}'. Check HazardTilemapConverter setup.", hazard);
            }

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

        SpriteRenderer sr = hazardObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.black; // Tạm thời đổi sang màu đen để nhận biết
        }
    }

    private bool TryConvertHazardTilemap(ElementalHazard hazard)
    {
        HazardTilemapConverter converter = hazard.GetComponent<HazardTilemapConverter>();
        if (converter == null)
        {
            return false;
        }

        Collider2D blockCollider = GetComponent<Collider2D>();
        Bounds conversionBounds = blockCollider != null
            ? blockCollider.bounds
            : new Bounds(transform.position, Vector3.one);

        return converter.TryConvertPool(conversionBounds, transform.position);
    }
}
