using System.Collections;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool pingPong = true;

    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitAtWaypointSeconds = 0.25f;

    private int _index;
    private int _dir = 1;
    private bool _waiting;

    private void FixedUpdate()
    {
        if (_waiting)
        {
            return;
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        if (_index < 0 || _index >= waypoints.Length || waypoints[_index] == null)
        {
            return;
        }

        Vector3 target = waypoints[_index].position;
        Vector3 next = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
        transform.position = next;

        if ((transform.position - target).sqrMagnitude <= 0.0001f)
        {
            AdvanceIndex();
            if (waitAtWaypointSeconds > 0f)
            {
                StartCoroutine(WaitRoutine());
            }
        }
    }

    private IEnumerator WaitRoutine()
    {
        _waiting = true;
        yield return new WaitForSeconds(waitAtWaypointSeconds);
        _waiting = false;
    }

    private void AdvanceIndex()
    {
        if (waypoints.Length <= 1)
        {
            _index = 0;
            return;
        }

        if (pingPong)
        {
            if (_index == waypoints.Length - 1)
            {
                _dir = -1;
            }
            else if (_index == 0)
            {
                _dir = 1;
            }

            _index += _dir;
        }
        else
        {
            _index = (_index + 1) % waypoints.Length;
        }
    }
}
