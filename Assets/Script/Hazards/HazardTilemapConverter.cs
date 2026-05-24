using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class HazardTilemapConverter : MonoBehaviour
{
    [Header("Source Tiles")]
    [SerializeField] private bool convertEveryTileInThisTilemap = true;
    [SerializeField] private List<TileBase> surfaceTiles = new List<TileBase>();
    [SerializeField] private List<TileBase> bodyTiles = new List<TileBase>();

    [Header("Obsidian Tiles")]
    [SerializeField] private Tilemap convertedTilemap;
    [SerializeField] private TileBase obsidianSurfaceTile;
    [SerializeField] private TileBase obsidianBodyTile;

    [Header("Solid Ground")]
    [SerializeField] private bool createSolidColliders = true;
    [SerializeField] private string solidColliderLayerName = "Ground";
    [SerializeField] private Transform solidColliderParent;

    [Header("Smoke")]
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private Sprite smokeSprite;
    [SerializeField] private List<Sprite> smokeFrames = new List<Sprite>();
    [SerializeField] private float smokeYOffset = 0.5f;
    [SerializeField] private float smokeLifetime = 1f;
    [SerializeField] private float smokeFrameRate = 12f;
    [SerializeField] private float smokeRiseSpeed = 0.6f;
    [SerializeField] private int smokeSortingOrder = 20;

    private Tilemap _tilemap;
    private readonly Queue<Vector3Int> _pendingCells = new Queue<Vector3Int>();
    private readonly HashSet<Vector3Int> _visitedCells = new HashSet<Vector3Int>();
    private readonly List<Vector3Int> _poolCells = new List<Vector3Int>();

    private static readonly Vector3Int[] NeighborOffsets =
    {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    public bool TryConvertPool(Bounds worldBounds)
    {
        return TryConvertPool(worldBounds, worldBounds.center);
    }

    public bool TryConvertPool(Bounds worldBounds, Vector3 preferredWorldPosition)
    {
        if (_tilemap == null)
        {
            _tilemap = GetComponent<Tilemap>();
        }

        if (convertedTilemap == null)
        {
            convertedTilemap = _tilemap;
        }

        Vector3Int startCell = FindHazardCell(worldBounds, preferredWorldPosition);
        if (!_tilemap.HasTile(startCell) || !IsConvertibleTile(_tilemap.GetTile(startCell)))
        {
            Debug.LogWarning($"Cannot convert '{name}' because no convertible hazard tile was found near {preferredWorldPosition}. Start cell was {startCell}.", this);
            return false;
        }

        CollectConnectedPool(startCell);
        if (_poolCells.Count == 0)
        {
            Debug.LogWarning($"Cannot convert '{name}' because the connected pool from {startCell} is empty.", this);
            return false;
        }

        if (obsidianSurfaceTile == null || obsidianBodyTile == null)
        {
            Debug.LogWarning($"Cannot convert '{name}' because obsidian tiles are not assigned.", this);
            return false;
        }

        for (int i = 0; i < _poolCells.Count; i++)
        {
            Vector3Int cell = _poolCells[i];
            TileBase sourceTile = _tilemap.GetTile(cell);
            TileBase replacementTile = ShouldUseSurfaceReplacement(cell, sourceTile)
                ? obsidianSurfaceTile
                : obsidianBodyTile;

            Vector3 worldCellCenter = _tilemap.GetCellCenterWorld(cell);
            Vector3Int convertedCell = convertedTilemap.WorldToCell(worldCellCenter);
            convertedTilemap.SetTile(convertedCell, replacementTile);

            if (convertedTilemap != _tilemap)
            {
                _tilemap.SetTile(cell, null);
            }

            if (createSolidColliders)
            {
                CreateSolidCollider(cell);
            }
        }

        SpawnSmoke(worldBounds.center);
        Debug.Log($"Converted {_poolCells.Count} tiles on '{name}' starting from {startCell}.", this);
        return true;
    }

    private Vector3Int FindHazardCell(Bounds worldBounds, Vector3 preferredWorldPosition)
    {
        Vector3Int minCell = _tilemap.WorldToCell(worldBounds.min);
        Vector3Int maxCell = _tilemap.WorldToCell(worldBounds.max);
        Vector3Int preferredCell = _tilemap.WorldToCell(preferredWorldPosition);

        if (_tilemap.HasTile(preferredCell) && IsConvertibleTile(_tilemap.GetTile(preferredCell)))
        {
            return preferredCell;
        }

        Vector3Int bestCell = preferredCell;
        float bestDistance = float.PositiveInfinity;

        for (int y = minCell.y - 1; y <= maxCell.y + 1; y++)
        {
            for (int x = minCell.x - 1; x <= maxCell.x + 1; x++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                TileBase tile = _tilemap.GetTile(cell);
                if (tile != null && IsConvertibleTile(tile))
                {
                    Vector3 cellCenter = _tilemap.GetCellCenterWorld(cell);
                    float distance = (cellCenter - preferredWorldPosition).sqrMagnitude;
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestCell = cell;
                    }
                }
            }
        }

        if (!float.IsPositiveInfinity(bestDistance))
        {
            return bestCell;
        }

        BoundsInt cellBounds = _tilemap.cellBounds;
        for (int y = cellBounds.yMin; y < cellBounds.yMax; y++)
        {
            for (int x = cellBounds.xMin; x < cellBounds.xMax; x++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                TileBase tile = _tilemap.GetTile(cell);
                if (tile == null || !IsConvertibleTile(tile))
                {
                    continue;
                }

                Vector3 cellCenter = _tilemap.GetCellCenterWorld(cell);
                float distance = (cellCenter - preferredWorldPosition).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestCell = cell;
                }
            }
        }

        return bestCell;
    }

    private void CollectConnectedPool(Vector3Int startCell)
    {
        _pendingCells.Clear();
        _visitedCells.Clear();
        _poolCells.Clear();

        _pendingCells.Enqueue(startCell);
        _visitedCells.Add(startCell);

        while (_pendingCells.Count > 0)
        {
            Vector3Int cell = _pendingCells.Dequeue();
            TileBase tile = _tilemap.GetTile(cell);
            if (tile == null || !IsConvertibleTile(tile))
            {
                continue;
            }

            _poolCells.Add(cell);

            for (int i = 0; i < NeighborOffsets.Length; i++)
            {
                Vector3Int neighbor = cell + NeighborOffsets[i];
                if (_visitedCells.Contains(neighbor))
                {
                    continue;
                }

                _visitedCells.Add(neighbor);
                _pendingCells.Enqueue(neighbor);
            }
        }
    }

    private bool ShouldUseSurfaceReplacement(Vector3Int cell, TileBase sourceTile)
    {
        if (surfaceTiles.Contains(sourceTile))
        {
            return true;
        }

        if (bodyTiles.Contains(sourceTile))
        {
            return false;
        }

        return IsSurfaceCell(cell);
    }

    private bool IsSurfaceCell(Vector3Int cell)
    {
        Vector3Int aboveCell = cell + Vector3Int.up;
        TileBase aboveTile = _tilemap.GetTile(aboveCell);
        return aboveTile == null || !IsConvertibleTile(aboveTile);
    }

    private bool IsConvertibleTile(TileBase tile)
    {
        if (tile == null)
        {
            return false;
        }

        if (convertEveryTileInThisTilemap || surfaceTiles.Count == 0 && bodyTiles.Count == 0)
        {
            return true;
        }

        return surfaceTiles.Contains(tile) || bodyTiles.Contains(tile);
    }

    private void SpawnSmoke(Vector3 worldPosition)
    {
        if (smokePrefab == null)
        {
            Sprite[] frames = GetSmokeFrames();
            if (frames.Length == 0)
            {
                return;
            }

            Vector3 spriteSpawnPosition = new Vector3(worldPosition.x, worldPosition.y + smokeYOffset, worldPosition.z);
            GameObject smokeObject = new GameObject("Smoke");
            smokeObject.transform.position = spriteSpawnPosition;

            SpriteRenderer smokeRenderer = smokeObject.AddComponent<SpriteRenderer>();
            smokeRenderer.sprite = frames[0];
            smokeRenderer.sortingOrder = smokeSortingOrder;

            SmokeSpriteAnimator smokeAnimator = smokeObject.AddComponent<SmokeSpriteAnimator>();
            smokeAnimator.Initialize(frames, smokeFrameRate, smokeLifetime, smokeRiseSpeed);
            return;
        }

        Vector3 prefabSpawnPosition = new Vector3(worldPosition.x, worldPosition.y + smokeYOffset, worldPosition.z);
        GameObject instance = Instantiate(smokePrefab, prefabSpawnPosition, Quaternion.identity);
        if (smokeLifetime > 0f)
        {
            Destroy(instance, smokeLifetime);
        }
    }

    private Sprite[] GetSmokeFrames()
    {
        if (smokeFrames.Count > 0)
        {
            return smokeFrames.FindAll(frame => frame != null).ToArray();
        }

        if (smokeSprite != null)
        {
            return new[] { smokeSprite };
        }

        return new Sprite[0];
    }

    private void CreateSolidCollider(Vector3Int sourceCell)
    {
        Vector3 worldCellCenter = _tilemap.GetCellCenterWorld(sourceCell);
        Vector2 colliderSize = GetWorldCellSize(sourceCell, worldCellCenter);

        GameObject colliderObject = new GameObject("ObsidianCollider");
        int colliderLayer = LayerMask.NameToLayer(solidColliderLayerName);
        colliderObject.layer = colliderLayer >= 0 ? colliderLayer : gameObject.layer;
        colliderObject.transform.position = worldCellCenter;

        if (solidColliderParent != null)
        {
            colliderObject.transform.SetParent(solidColliderParent, true);
        }

        BoxCollider2D collider = colliderObject.AddComponent<BoxCollider2D>();
        collider.size = colliderSize;
        collider.isTrigger = false;
    }

    private Vector2 GetWorldCellSize(Vector3Int sourceCell, Vector3 worldCellCenter)
    {
        Vector3 rightCenter = _tilemap.GetCellCenterWorld(sourceCell + Vector3Int.right);
        Vector3 upCenter = _tilemap.GetCellCenterWorld(sourceCell + Vector3Int.up);

        float width = Mathf.Abs(rightCenter.x - worldCellCenter.x);
        float height = Mathf.Abs(upCenter.y - worldCellCenter.y);

        if (width <= 0.001f)
        {
            width = 1f;
        }

        if (height <= 0.001f)
        {
            height = 1f;
        }

        return new Vector2(width, height);
    }
}
