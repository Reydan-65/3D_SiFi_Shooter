using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ImpactEffectBase : MonoBehaviour
{
    [SerializeField] protected float m_LifeTime;

    protected float m_Timer;

    protected virtual void FixedUpdate()
    {
        if (m_Timer < m_LifeTime)
            m_Timer += Time.deltaTime;
        else
            Destroy(gameObject);
    }
}
