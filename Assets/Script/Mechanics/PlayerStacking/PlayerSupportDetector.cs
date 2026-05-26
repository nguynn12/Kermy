using UnityEngine;

public class PlayerSupportDetector : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float probeHeight = 0.08f;
    [SerializeField] private float probeWidthFactor = 0.5f;

    public bool IsSupportingPlayer { get; private set; }

    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        IsSupportingPlayer = CheckSupportingNow();
    }

    public bool CheckSupportingNow()
    {
        if (_col == null)
        {
            return false;
        }

        Bounds b = _col.bounds;
        Vector2 probeCenter = new Vector2(b.center.x, b.max.y + (probeHeight * 0.5f));
        Vector2 probeSize = new Vector2(b.size.x * Mathf.Clamp01(probeWidthFactor), probeHeight);

        Collider2D hit = Physics2D.OverlapBox(probeCenter, probeSize, 0f, ResolvePlayerLayer());
        if (hit == null)
        {
            return false;
        }

        return hit.gameObject != gameObject;
    }

    private LayerMask ResolvePlayerLayer()
    {
        if (playerLayer.value != 0)
        {
            return playerLayer;
        }

        int playerLayerIndex = LayerMask.NameToLayer("Player");
        return playerLayerIndex >= 0 ? 1 << playerLayerIndex : Physics2D.AllLayers;
    }
}
