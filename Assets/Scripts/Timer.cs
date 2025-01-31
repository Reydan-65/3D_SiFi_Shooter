using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    private static GameObject TimerCollector;

    public event UnityAction OnTimerRunOut;
    public event UnityAction OnTick;

    public bool IsLoop;

    private float m_maxTime;
    private float m_CurrentTime;
    private bool isPause;

    public bool IsFinished => m_CurrentTime <= 0;
    public bool IsPause => isPause;

    private void Update()
    {
        if (isPause) return;

        m_CurrentTime -= Time.deltaTime;

        if (OnTick != null) OnTick.Invoke();

        if (m_CurrentTime <= 0)
        {
            m_CurrentTime = 0;

            if (OnTimerRunOut != null) OnTimerRunOut.Invoke();

            if (IsLoop)
            {
                m_CurrentTime = m_maxTime;
            }
        }
    }

    public static Timer CreateTimer(float time, bool isLoop)
    {
        if (TimerCollector == null)
        {
            TimerCollector = new GameObject("Timers");
        }

        Timer timer = TimerCollector.AddComponent<Timer>();

        timer.m_maxTime = time;
        timer.IsLoop = isLoop;

        return timer;
    }

    public void Play()
    {
        isPause = false;
    }

    public void Pause()
    {
        isPause = true;
    }

    public void Completed()
    {
        isPause = false;

        m_CurrentTime = 0;
    }

    public void StopWithoutEvent()
    {
        Destroy(this);
    }

    public void Restart(float time)
    {
        m_maxTime = time;
        m_CurrentTime = m_maxTime;
    }

    public void Restart()
    {
        m_CurrentTime = m_maxTime;
    }
}
