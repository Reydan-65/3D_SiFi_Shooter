using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ImpactEffectBase : MonoBehaviour
{
    [SerializeField] protected float m_LifeTime;

    protected float m_Timer;

    // Звук слегка изменяется
    private void Start()
    {
        //float pitch = Random.Range(0.95f, 1.05f);
        //transform.root.GetComponent<AudioSource>().pitch = pitch;
    }

    protected virtual void FixedUpdate()
    {
        if (m_Timer < m_LifeTime)
            m_Timer += Time.deltaTime;
        else
            Destroy(gameObject);
    }
}
