using UnityEngine;

public abstract class ProjectileBase : Entity
{
    [SerializeField] protected float m_Velocity;
    [SerializeField] protected float m_Lifetime;
    [SerializeField] protected int m_Damage;

    [Space(10)]
    [Header("Disabler = Null (if not requared)")]
    [SerializeField] protected GameObject m_DisablerPrefab;

    protected virtual void OnHit(Destructible destructible) { }
    protected virtual void OnHit(Collider collider2D) { }
    protected virtual void OnProjectileLifeEnd(Collider collider, Vector3 position, Vector3 normal) { }

    public float Velocity => m_Velocity;
    protected float m_Timer;

    protected Destructible m_Parent;

    protected virtual void Awake() { }

    protected virtual void FixedUpdate()
    {
        float stepLenght = Time.deltaTime * m_Velocity;

        Vector3 step = transform.forward * stepLenght;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, stepLenght) == true)
        {
            OnHit(hit.collider);

            Destructible destructible = hit.collider.transform.root.GetComponent<Destructible>();

            if (destructible != null && destructible != m_Parent)
            {
                destructible.ApplyDamage(m_Damage);

                OnHit(destructible);
                OnProjectileLifeEnd(hit.collider, hit.point, hit.normal);
            }

            hit = OnHitObstacles(hit);
        }

        // Время жизни снаряда.
        m_Timer += Time.deltaTime;

        if (m_Timer > m_Lifetime)
        {
            if (hit.collider != null)
                OnProjectileLifeEnd(hit.collider, hit.point, hit.normal);
            else
                Destroy(gameObject);
        }

        transform.position += new Vector3(step.x, step.y, step.z);
    }

    // Попадание в препятствие
    protected virtual RaycastHit OnHitObstacles(RaycastHit hit)
    {
        return hit;
    }

    // Указание на того, кто выпустил снаряд.
    public void SetParentShooter(Destructible parent)
    {
        m_Parent = parent;
    }
}
