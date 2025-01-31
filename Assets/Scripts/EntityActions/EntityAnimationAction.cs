using UnityEngine;

public class EntityAnimationAction : EntityAction
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private string m_ActionAnimationName;
    [SerializeField] private float m_TimeDuration;

    private Timer m_Timer;
    private bool IsPlayingAnimation;

    public override void StartAction()
    {
        base.StartAction();

        m_Animator.CrossFade(m_ActionAnimationName, m_TimeDuration);

        m_Timer = Timer.CreateTimer(m_TimeDuration, true);
        m_Timer.OnTick += OnTimerTick;
    }

    public override void EndAction()
    {
        base.EndAction();
        m_Timer.OnTick -= OnTimerTick;
    }

    private void OnTimerTick()
    {
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_ActionAnimationName) == true)
        {
            IsPlayingAnimation = true;
        }

        if (IsPlayingAnimation == true)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_ActionAnimationName) == false)
            {
                IsPlayingAnimation = false;

                EndAction();
            }
        }
    }
}
