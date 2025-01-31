using UnityEngine;
using UnityEngine.Events;

public abstract class EntityActionProperties { }

public abstract class EntityAction : MonoBehaviour
{
    [SerializeField] private UnityEvent m_EventOnStart;
    [SerializeField] private UnityEvent m_EventOnEnd;

    private EntityActionProperties m_Properties;
    public EntityActionProperties Properties => m_Properties;

    private bool isStarted;

    public UnityEvent EventOnStart => m_EventOnStart;
    public UnityEvent EventOnEnd => m_EventOnEnd;

    public virtual void StartAction()
    {
        if (isStarted) return;

        isStarted = true;
        m_EventOnStart?.Invoke();
    }

    public virtual void EndAction()
    {
        isStarted = false;
        m_EventOnEnd?.Invoke();
    }

    public virtual void SetProperties(EntityActionProperties properties)
    {
        m_Properties = properties;
    }
}
