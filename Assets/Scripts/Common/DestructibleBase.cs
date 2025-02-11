using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// ������������ ������ �� �����. �� ��� ����� ����� ���������.
/// </summary>
public class DestructibleBase : Entity
{
    [SerializeField] protected UnityEvent m_EventOnDeath;
    public UnityEvent EventOnDeath => m_EventOnDeath;

    [SerializeField] protected UnityEvent m_EventOnGetDamage;
    public UnityAction<DestructibleBase> OnGetDamage;

    protected bool isDead;
    public bool IsDead => isDead;

    #region Properties

    /// <summary>
    /// ������ ���������� �����������.
    /// </summary>
    [SerializeField] protected bool m_Indestructible;
    public bool IsIndestructible { get => m_Indestructible; set => m_Indestructible = value; }

    /// <summary>
    /// ��������� ���������� ����������.
    /// </summary>
    [SerializeField] protected int m_HitPoints;
    public int MaxHitPoints => m_HitPoints;

    /// <summary>
    /// ������� ���������.
    /// </summary>
    protected int m_CurrentHitPoints;
    public int HitPoints => m_CurrentHitPoints;

    #endregion

    #region Unity Events 

    protected virtual void Start()
    {
        transform.SetParent(null);
        isDead = false;
        m_CurrentHitPoints = m_HitPoints;
    }

    #endregion

    #region Public API

    /// <summary>
    /// ���������� ����� � �������.
    /// </summary>
    /// <param name="damage"> ���� ��������� ������� </param>
    public void ApplyDamage(int damage, Destructible other)
    {
        if (m_Indestructible || isDead) return;
        
        m_CurrentHitPoints -= damage;

        if (m_CurrentHitPoints <= 0)
        {
            m_CurrentHitPoints = 0;
            isDead = true;
            OnDeath();
        }

        OnGetDamage?.Invoke(other);
        m_EventOnGetDamage?.Invoke();
    }

    public void ApplyHeal(int count)
    {
        if (isDead) return;

        m_CurrentHitPoints += count;
        if (m_CurrentHitPoints >= m_HitPoints) m_CurrentHitPoints = m_HitPoints;
    }

    public void RestoreHealth()
    {
        if (isDead) return;

        m_CurrentHitPoints = m_HitPoints;
    }
    #endregion

    /// <summary>
    /// ���������������� ������� ����������� �������, ����� ��������� ����� ����
    /// </summary>
    protected virtual void OnDeath()
    {
        m_EventOnDeath?.Invoke();
    }

    #region DESTRUCTIBLE_LIST_TEAMS

    [SerializeField] protected int m_TeamId;

    public int TeamId { get => m_TeamId; set => m_TeamId = value; }

    /// <summary>
    /// �������� ���� ����������� �������� � ������.
    /// �������� �� ������� ������, ���� ������ ���������.
    /// ���������� �� ������.
    /// </summary>
    protected static HashSet<DestructibleBase> m_AllDestructible;
    public static IReadOnlyCollection<DestructibleBase> AllDestructible => m_AllDestructible;

    protected virtual void OnEnable()
    {
        if (m_AllDestructible == null)
            m_AllDestructible = new HashSet<DestructibleBase>();

        m_AllDestructible.Add(this);
    }

    protected virtual void OnDestroy()
    {
        m_AllDestructible.Remove(this);
    }

    public const int TeamId_Neutral = 0;
    public const int TeamId_Allies = 1;
    public const int TeamId_Enemies = 2;
    public const int TeamId_Debris = 3;

    #endregion

}
