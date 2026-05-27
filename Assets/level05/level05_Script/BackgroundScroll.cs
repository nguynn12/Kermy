using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 3.2f;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float coveragePadding = 2f;

    private readonly List<Transform> segments = new List<Transform>();
    private SpriteRenderer sourceRenderer;
    private float segmentWidth;
    private float leftEdge;
    private float rightEdge;

    private void Awake()
    {
        sourceRenderer = GetComponent<SpriteRenderer>();
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null || sourceRenderer.sprite == null)
        {
            enabled = false;
            return;
        }

        SetupBaseSegment();
        BuildSegments();
    }

    private void Update()
    {
        float step = scrollSpeed * Time.deltaTime;
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].position += Vector3.left * step;
        }

        for (int i = 0; i < segments.Count; i++)
        {
            Transform segment = segments[i];
            if (segment.position.x + segmentWidth * 0.5f < leftEdge)
            {
                float farRight = FindRightMostX();
                segment.position = new Vector3(farRight + segmentWidth, segment.position.y, segment.position.z);
            }
        }
    }

    private void SetupBaseSegment()
    {
        float cameraHeight = targetCamera.orthographicSize * 2f;
        float spriteHeight = sourceRenderer.sprite.bounds.size.y;
        float scale = (cameraHeight + coveragePadding) / spriteHeight;

        transform.localScale = new Vector3(scale, scale, 1f);
        transform.position = new Vector3(targetCamera.transform.position.x, targetCamera.transform.position.y, transform.position.z);

        segmentWidth = sourceRenderer.bounds.size.x;
        float cameraWidth = cameraHeight * targetCamera.aspect;
        leftEdge = targetCamera.transform.position.x - cameraWidth * 0.5f - coveragePadding;
        rightEdge = targetCamera.transform.position.x + cameraWidth * 0.5f + coveragePadding;
    }

    private void BuildSegments()
    {
        segments.Clear();

        int segmentCount = Mathf.CeilToInt((rightEdge - leftEdge) / segmentWidth) + 2;
        float startX = targetCamera.transform.position.x - segmentWidth;

        for (int i = 0; i < segmentCount; i++)
        {
            Transform segment = i == 0 ? transform : CreateSegment(i).transform;
            segment.position = new Vector3(startX + segmentWidth * i, targetCamera.transform.position.y, transform.position.z);
            segments.Add(segment);
        }
    }

    private GameObject CreateSegment(int index)
    {
        GameObject segment = new GameObject($"{name}_Loop_{index}");
        segment.transform.SetParent(transform.parent);
        segment.transform.localScale = transform.localScale;

        SpriteRenderer renderer = segment.AddComponent<SpriteRenderer>();
        renderer.sprite = sourceRenderer.sprite;
        renderer.color = sourceRenderer.color;
        renderer.sortingLayerID = sourceRenderer.sortingLayerID;
        renderer.sortingOrder = sourceRenderer.sortingOrder;
        renderer.material = sourceRenderer.sharedMaterial;

        return segment;
    }

    private float FindRightMostX()
    {
        float farRight = float.MinValue;
        for (int i = 0; i < segments.Count; i++)
        {
            farRight = Mathf.Max(farRight, segments[i].position.x);
        }

        return farRight;
    }
}
