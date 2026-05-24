using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class MenuScrollingBackground : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.018f, 0f);

    private RawImage backgroundImage;
    private Rect uvRect;
    private Vector2 lastRectSize;

    public void SetScrollSpeed(Vector2 speed)
    {
        scrollSpeed = speed;
    }

    private void Awake()
    {
        backgroundImage = GetComponent<RawImage>();

        if (backgroundImage.texture != null)
        {
            backgroundImage.texture.wrapMode = TextureWrapMode.Repeat;
        }
    }

    private void OnEnable()
    {
        FitTextureToViewport();
    }

    private void Update()
    {
        RectTransform rectTransform = (RectTransform)transform;
        if (rectTransform.rect.size != lastRectSize)
        {
            FitTextureToViewport();
        }

        uvRect.position -= scrollSpeed * Time.unscaledDeltaTime;
        backgroundImage.uvRect = uvRect;
    }

    private void FitTextureToViewport()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<RawImage>();
        }

        Texture texture = backgroundImage.texture;
        RectTransform rectTransform = (RectTransform)transform;
        lastRectSize = rectTransform.rect.size;

        if (texture == null || lastRectSize.x <= 0f || lastRectSize.y <= 0f)
        {
            uvRect = backgroundImage.uvRect;
            return;
        }

        float screenAspect = lastRectSize.x / lastRectSize.y;
        float textureAspect = (float)texture.width / texture.height;

        uvRect = backgroundImage.uvRect;
        // Show only one screen-sized window into the wide panorama, then scroll that window forever.
        uvRect.width = screenAspect / textureAspect;
        uvRect.height = 1f;
        uvRect.y = 0f;
        backgroundImage.uvRect = uvRect;
    }
}
