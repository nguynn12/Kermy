using System.Collections;
using UnityEngine;

public class TimerToggle : MonoBehaviour
{
    [SerializeField] private LaserBarrier target;

    [Header("Cycle")]
    [SerializeField] private float onSeconds = 2f;
    [SerializeField] private float offSeconds = 2f;
    [SerializeField] private bool startOn = true;

    private Coroutine _routine;

    private void OnEnable()
    {
        if (target == null)
        {
            return;
        }

        _routine = StartCoroutine(Run());
    }

    private void OnDisable()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    private IEnumerator Run()
    {
        bool state = startOn;

        while (true)
        {
            if (target != null)
            {
                target.SetState(state);
            }

            float wait = state ? onSeconds : offSeconds;
            if (wait < 0f)
            {
                wait = 0f;
            }

            yield return new WaitForSeconds(wait);
            state = !state;
        }
    }
}
