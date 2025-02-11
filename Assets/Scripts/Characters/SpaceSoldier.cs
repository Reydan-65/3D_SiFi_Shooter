using UnityEngine;

public class SpaceSoldier : Destructible
{
    // Actions
    [SerializeField] private EntityAnimationAction m_ActionDeath;

    // Audio
    [SerializeField] private AudioClip[] m_audioClips;
    private AudioSource m_audioSource;
    public AudioSource AudioSource => m_audioSource;
    public AudioClip[] AudioClips => m_audioClips;

    protected override void Start()
    {
        base.Start();

        DetectionIndicator.Instance.SetVisible(false);

        m_audioSource = GetComponent<AudioSource>();
    }

    protected override void OnDeath()
    {
        EventOnDeath?.Invoke();
        m_ActionDeath.StartAction();
    }
}
