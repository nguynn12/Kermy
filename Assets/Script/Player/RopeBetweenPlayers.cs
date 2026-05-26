using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeBetweenPlayers : MonoBehaviour
{
    [Header("Hai nhân vật")]
    public Transform playerFire;
    public Transform playerWater;

    [Header("Giới hạn dây")]
    public float maxDistance = 4.5f;
    public float pullForce = 25f;
    public float maxPullSpeed = 6f;

    [Header("Hiển thị dây")]
    public LineRenderer ropeLine;
    public float ropeWidth = 0.08f;
    public int sortingOrder = 50;

    [Header("Texture dây")]
    public Texture2D ropeTexture;
    public float textureRepeatPerUnit = 5f;
    public float textureScrollSpeed = 0.3f;

    private Material ropeMaterial;
    private Rigidbody2D rbFire;
    private Rigidbody2D rbWater;

    void Awake()
    {
        ropeLine = GetComponent<LineRenderer>();
        SetupLineRenderer();

        rbFire = playerFire.GetComponent<Rigidbody2D>();
        rbWater = playerWater.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (rbFire == null || rbWater == null)
            return;

        LimitDistanceSmartAnchor();
    }

    void LateUpdate()
    {
        if (playerFire == null || playerWater == null || ropeLine == null)
            return;

        DrawRope();
        AnimateRopeTexture();
    }

    void LimitDistanceSmartAnchor()
    {
        Vector2 firePos = rbFire.position;
        Vector2 waterPos = rbWater.position;

        Vector2 direction = waterPos - firePos;
        float distance = direction.magnitude;

        // Nếu khoảng cách vẫn nằm trong giới hạn cho phép -> Không can thiệp vật lý
        if (distance <= maxDistance)
            return;

        Vector2 normal = direction.normalized;
        float excess = distance - maxDistance;

        bool fireFalling = rbFire.linearVelocity.y < -0.2f;
        bool waterFalling = rbWater.linearVelocity.y < -0.2f;

        // Trường hợp 1: Nếu Water đang rơi tự do, Fire đứng im làm neo -> kéo Water lại
        if (waterFalling && !fireFalling)
        {
            PullBodyToPoint(rbWater, firePos + normal * maxDistance, excess);
            return;
        }

        // Trường hợp 2: Nếu Fire đang rơi tự do, Water đứng im làm neo -> kéo Fire lại
        if (fireFalling && !waterFalling)
        {
            PullBodyToPoint(rbFire, waterPos - normal * maxDistance, excess);
            return;
        }

        // =========================================================================
        // 🌟 ĐOẠN FIX DỨT ĐIỂM LỖI NHẢY KÉO: 
        // Nếu một trong hai đứa đang lao lên trời (Nhảy), đứa còn lại ĐANG ĐỨNG TRÊN ĐẤT 
        // thì TUYỆT ĐỐI không được ép vị trí hay cộng vận tốc giật cục để tránh bị hất bay theo.
        // =========================================================================
        if (rbWater.linearVelocity.y > 0.5f && Mathf.Abs(rbFire.linearVelocity.y) < 0.1f)
        {
            // Con Nước đang nhảy lên, con Lửa đang đứng yên -> Sợi dây chỉ căng ra giữ con Nước lại không cho đi quá xa
            rbWater.position = firePos + normal * maxDistance;
            rbWater.linearVelocity = new Vector2(rbWater.linearVelocity.x, Mathf.Min(rbWater.linearVelocity.y, maxPullSpeed));
            return;
        }

        if (rbFire.linearVelocity.y > 0.5f && Mathf.Abs(rbWater.linearVelocity.y) < 0.1f)
        {
            // Con Lửa đang nhảy lên, con Nước đang đứng yên -> Sợi dây chỉ căng ra giữ con Lửa lại
            rbFire.position = waterPos - normal * maxDistance;
            rbFire.linearVelocity = new Vector2(rbFire.linearVelocity.x, Mathf.Min(rbFire.linearVelocity.y, maxPullSpeed));
            return;
        }

        // Nếu rơi vào các trường hợp đu dây thông thường khác (cả 2 cùng bay, cùng rơi...)
        Vector2 correction = normal * excess * 0.5f;
        rbFire.position += correction;
        rbWater.position -= correction;

        ClampVelocityAlongRope();
    }

    void PullBodyToPoint(Rigidbody2D body, Vector2 targetPos, float excess)
    {
        Vector2 toTarget = targetPos - body.position;

        Vector2 pullVelocity = toTarget.normalized * Mathf.Min(maxPullSpeed, excess * pullForce * Time.fixedDeltaTime);

        body.linearVelocity = new Vector2(
            pullVelocity.x,
            Mathf.Max(body.linearVelocity.y, pullVelocity.y)
        );

        if (Vector2.Distance(body.position, targetPos) < 0.05f)
        {
            body.position = targetPos;
        }
    }

    void ClampVelocityAlongRope()
    {
        Vector2 firePos = rbFire.position;
        Vector2 waterPos = rbWater.position;

        Vector2 normal = (waterPos - firePos).normalized;
        Vector2 relativeVelocity = rbWater.linearVelocity - rbFire.linearVelocity;

        float separatingSpeed = Vector2.Dot(relativeVelocity, normal);

        if (separatingSpeed > 0f)
        {
            Vector2 correction = normal * separatingSpeed * 0.5f;
            rbFire.linearVelocity += correction;
            rbWater.linearVelocity -= correction;
        }
    }

    void SetupLineRenderer()
    {
        ropeLine.positionCount = 2;
        ropeLine.useWorldSpace = true;
        ropeLine.startWidth = ropeWidth;
        ropeLine.endWidth = ropeWidth;
        ropeLine.textureMode = LineTextureMode.Tile;
        ropeLine.alignment = LineAlignment.View;
        ropeLine.sortingLayerName = "Default";
        ropeLine.sortingOrder = sortingOrder;

        Shader shader = Shader.Find("Sprites/Default");
        ropeMaterial = new Material(shader);

        if (ropeTexture != null)
            ropeMaterial.mainTexture = ropeTexture;

        ropeLine.material = ropeMaterial;
    }

    void DrawRope()
    {
        Vector3 firePos = playerFire.position;
        Vector3 waterPos = playerWater.position;

        firePos.z = -1f;
        waterPos.z = -1f;

        ropeLine.SetPosition(0, firePos);
        ropeLine.SetPosition(1, waterPos);
    }

    void AnimateRopeTexture()
    {
        if (ropeLine.material == null || ropeTexture == null)
            return;

        float distance = Vector2.Distance(playerFire.position, playerWater.position);

        ropeLine.material.mainTextureScale = new Vector2(distance * textureRepeatPerUnit, 1f);
        ropeLine.material.mainTextureOffset = new Vector2(Time.time * textureScrollSpeed, 0f);
    }
}