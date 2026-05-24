using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopePhysics : MonoBehaviour
{
    [Header("Bodies")]
    [SerializeField] private Rigidbody2D rbB;

    [Header("Rope")]
    [SerializeField] private bool autoSetLengthOnStart = true;
    [SerializeField] private float ropeLength = 3.5f;
    [SerializeField] private float extraSlack = 0.5f;

    [Header("Joint")]
    [SerializeField] private bool enableCollisionBetweenPlayers = true;

    private Rigidbody2D _rbA;
    private DistanceJoint2D _joint;

    private void Awake()
    {
        _rbA = GetComponent<Rigidbody2D>();
        _joint = GetComponent<DistanceJoint2D>();
        if (_joint == null)
        {
            _joint = gameObject.AddComponent<DistanceJoint2D>();
        }
    }

    private void Start()
    {
        if (_rbA == null || rbB == null)
        {
            Debug.LogError("RopePhysics: Chưa gán rbB hoặc thiếu Rigidbody2D trên object chứa RopePhysics.");
            enabled = false;
            return;
        }

        ConfigureJoint();

        if (autoSetLengthOnStart)
        {
            float dist = Vector2.Distance(_rbA.position, rbB.position);
            _joint.distance = dist + Mathf.Max(0f, extraSlack);
        }
        else
        {
            _joint.distance = Mathf.Max(0.01f, ropeLength);
        }
    }

    private void ConfigureJoint()
    {
        _joint.connectedBody = rbB;
        _joint.enableCollision = enableCollisionBetweenPlayers;

        _joint.autoConfigureDistance = false;
        _joint.maxDistanceOnly = true;

        _joint.autoConfigureConnectedAnchor = false;
        _joint.anchor = Vector2.zero;
        _joint.connectedAnchor = Vector2.zero;
    }

    public void SetRopeLength(float length)
    {
        ropeLength = Mathf.Max(0.01f, length);
        if (_joint != null)
        {
            _joint.distance = ropeLength;
        }
    }
}
