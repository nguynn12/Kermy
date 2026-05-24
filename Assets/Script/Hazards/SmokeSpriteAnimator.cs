using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SmokeSpriteAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameRate = 12f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float riseSpeed = 0.6f;

    private SpriteRenderer _spriteRenderer;
    private float _age;

    public void Initialize(Sprite[] animationFrames, float animationFrameRate, float animationLifetime, float animationRiseSpeed)
    {
        frames = animationFrames;
        frameRate = animationFrameRate;
        lifetime = animationLifetime;
        riseSpeed = animationRiseSpeed;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _age += Time.deltaTime;

        if (frames != null && frames.Length > 0)
        {
            int frameIndex = Mathf.Min(Mathf.FloorToInt(_age * frameRate), frames.Length - 1);
            _spriteRenderer.sprite = frames[frameIndex];
        }

        transform.position += Vector3.up * (riseSpeed * Time.deltaTime);

        if (lifetime > 0f && _age >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
