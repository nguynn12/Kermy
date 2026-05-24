using UnityEngine;

public class ConnectingRope : MonoBehaviour
{
    [Header("Nhân vật cần nối")]
    [SerializeField] private Transform characterA;
    [SerializeField] private Transform characterB;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();

        if (line == null)
        {
            Debug.LogError("Thiếu LineRenderer!");
            return;
        }

        line.positionCount = 2; // chỉ set 1 lần
    }

    void Update()
    {
        if (characterA == null || characterB == null) return;

        line.SetPosition(0, characterA.position);
        line.SetPosition(1, characterB.position);
    }
}
