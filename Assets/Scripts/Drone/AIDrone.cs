using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Drone))]
public class AIDrone : MonoBehaviour
{
    [SerializeField] private ColliderViewer m_ColliderViewer;
    [SerializeField] private float minDistanceToTarget;

    private Drone m_Drone;
    private Vector3 m_MovementPosition;
    private GameObject m_ShootTargetObject;
    private Vector3 m_ShootTarget;
    private CubeArea m_MovementArea;
    public CubeArea MovementArea { get => m_MovementArea; set => m_MovementArea = value; }

    // Unity Events
    private void Start()
    {
        m_Drone = GetComponent<Drone>();
        m_Drone.EventOnDeath.AddListener(OnDroneDeath);
        m_Drone.OnGetDamage += OnGetDamage;
    }

    private void Update()
    {
        UpdateAI();
    }

    private void OnDestroy()
    {
        m_Drone.EventOnDeath.RemoveListener(OnDroneDeath);
        m_Drone.OnGetDamage -= OnGetDamage;
    }

    // Handlers
    private void OnDroneDeath()
    {
        if (m_ShootTarget != null && m_ShootTargetObject != null && IsPlayerTarget(m_ShootTargetObject.gameObject))
        {
            if (PlayerVisibilityManager.Instance != null)
            {
                PlayerVisibilityManager.Instance.RegisterVisibility(false, null, this);
            }

            if (DetectionIndicator.Instance != null)
            {
                DetectionIndicator.Instance.SetVisible(false);
            }
        }

            enabled = false;
    }

    private void OnGetDamage(DestructibleBase other)
    {
        if (!other.IsDead && other.TeamId != m_Drone.TeamId)
        {
            ActionAssingTargetAllTeamMember(other.transform);
        }
    }

    // UpdateAI
    private void UpdateAI()
    {
        // Проверяем, жива ли текущая цель
        if (m_ShootTargetObject != null)
        {
            Destructible targetDestructible = m_ShootTargetObject.GetComponent<Destructible>();
            if (targetDestructible != null && targetDestructible.IsDead)
            {
                // Если цель мертва, сбрасываем цель
                SetShootTarget(null);
            }
        }

        // Target Detection
        ActionFindShootTarget();

        // Move
        ActionMove();

        // Fire
        ActionFire();
    }

    private bool IsPlayerTarget(GameObject target)
    {
        return target.CompareTag("Player");
    }

    private bool IsFacingTarget(Vector3 target)
    {
        Vector3 directionToTarget = (target - m_Drone.MainMesh.position).normalized;
        float dotProduct = Vector3.Dot(-m_Drone.MainMesh.forward, directionToTarget);
        return dotProduct < -0.99f;
    }

    private void ActionFindShootTarget()
    {
        Transform potentialTarget = FindShootTarget();

        if (potentialTarget != null)
        {
            SetShootTarget(potentialTarget);
            if (m_ShootTarget != null && IsPlayerTarget(m_ShootTargetObject.gameObject))
            {
                if (m_ColliderViewer.IsObjectVisible(m_ShootTargetObject))
                {
                    PlayerVisibilityManager.Instance.RegisterVisibility(true, null, this);
                }
            }
            m_Drone.LookAt(m_ShootTarget);
            ActionAssingTargetAllTeamMember(potentialTarget);
        }
    }

    private void ActionMove()
    {
        if (m_ShootTarget != Vector3.zero)
        {
            // Если цель есть, следим за ней
            m_Drone.LookAt(m_ShootTarget);

            // Проверяем расстояние до цели
            float distanceToTarget = Vector3.Distance(transform.position, m_ShootTarget);

            if (distanceToTarget > minDistanceToTarget)
            {
                // Если цель дальше минимальной дистанции, двигаемся к ней
                m_Drone.StartMoving();
                m_Drone.MoveAt(m_ShootTarget);
            }
            else
            {
                // Если цель ближе минимальной дистанции, останавливаемся
                m_Drone.StopMoving();

                // Производим поиск новой цели
                Transform newTarget = FindShootTarget();

                if (newTarget != null)
                {
                    // Если новая цель найдена, устанавливаем её
                    SetShootTarget(newTarget);
                }
                else
                {
                    // Если цель не обнаружена, возвращаемся в область движения
                    if (m_ShootTarget != null && IsPlayerTarget(m_ShootTargetObject.gameObject))
                    {
                        PlayerVisibilityManager.Instance.RegisterVisibility(false, null, this);
                        DetectionIndicator.Instance.SetVisible(false);
                    }

                    SetShootTarget(null);

                    if (m_MovementArea != null)
                        m_MovementPosition = m_MovementArea.GetRandomInsideZone();

                    m_Drone.StartMoving();
                    m_Drone.MoveAt(m_MovementPosition);
                    m_Drone.LookAt(m_MovementPosition);
                }
            }
        }
        else
        {
            m_Drone.StartMoving();
            if (m_MovementArea != null)
            {
                if (Vector3.Distance(transform.position, m_MovementPosition) < 0.1f)
                {
                    m_MovementPosition = m_MovementArea.GetRandomInsideZone();
                }

                if (Physics.Linecast(m_Drone.MainMesh.position, m_MovementPosition))
                {
                    m_MovementPosition = m_MovementArea.GetRandomInsideZone();
                }

                m_Drone.MoveAt(m_MovementPosition);
                m_Drone.LookAt(m_MovementPosition);
            }
        }
    }

    private void ActionFire()
    {
        if (m_ShootTarget != Vector3.zero && IsFacingTarget(m_ShootTarget))
        {
            if (m_ShootTargetObject != null && m_ColliderViewer.IsObjectVisible(m_ShootTargetObject))
            {
                if (m_ShootTargetObject.GetComponent<Destructible>().IsDead == false)
                    m_Drone.Fire(m_ShootTarget);
            }
        }
    }

    // Methods
    public void SetShootTarget(Transform target)
    {
        if (target != null)
        {
            m_ShootTargetObject = target.gameObject;
            m_ShootTarget = new Vector3(target.transform.position.x, target.transform.position.y + 0.9f, target.transform.position.z);
        }
        else
        {
            m_ShootTargetObject = null;
            m_ShootTarget = Vector3.zero;
        }
    }

    private Transform FindShootTarget()
    {
        List<Destructible> targets = Destructible.GetAllNonTeamMembers(m_Drone.TeamId);

        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].IsDead && m_ColliderViewer.IsObjectVisible(targets[i].gameObject) == true)
                return targets[i].transform;
        }

        return null;
    }

    private void ActionAssingTargetAllTeamMember(Transform other)
    {
        if (other.transform.root.GetComponent<Destructible>().IsDead == true) return;

        List<Destructible> team = Destructible.GetAllTeamMembers(m_Drone.TeamId);

        foreach (Destructible dest in team)
        {
            AIDrone ai = dest.transform.root.GetComponent<AIDrone>();

            if (ai != null && ai.enabled == true)
            {
                ai.SetShootTarget(other);
            }
        }
    }
}
