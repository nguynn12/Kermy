using UnityEngine;

public class RopePhysicsSoft : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbA;
    [SerializeField] private Rigidbody2D rbB;

    private DistanceJoint2D joint;

    void Start()
    {
        if (rbA == null || rbB == null)
        {
            Debug.LogError("Chưa gán Rigidbody!");
            return;
        }

        if (joint == null)
        {
            joint = gameObject.AddComponent<DistanceJoint2D>();
        }

        joint.enableCollision = true;
        joint.connectedBody = rbB;

        joint.autoConfigureDistance = false;
        joint.maxDistanceOnly = true;

        joint.distance = Vector2.Distance(rbA.position, rbB.position);
    }
}
