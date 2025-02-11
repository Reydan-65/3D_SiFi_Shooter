using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAlienSoldier : MonoBehaviour
{
    public enum AIBehaviour
    {
        Null,
        Idle,
        PatrolRandom,
        PatrolCircle,
        PursuetTarget,
        SeekTarget
    }

    [SerializeField] private AIBehaviour m_AIBehaviour;
    [SerializeField] private AlienSoldier m_AlienSoldier;
    [SerializeField] private CharacterMovement m_characterMovement;
    [SerializeField] private NavMeshAgent m_Agent;
    [SerializeField] private PatrolPath m_PatrolPath;
    [SerializeField] private int m_PathNodeIndex = 0;

    [SerializeField] private ColliderViewer m_ColliderViewer;
    [SerializeField] private float m_AimingDistance;
    [SerializeField] private float m_MinDistanceToTarget;
    [SerializeField] private float findTargetTimer;

    [Header("Sound Heard")]
    [SerializeField] private float m_HearingRange;
    [SerializeField] private float m_SoundInvestigationTime;
    [SerializeField] private float m_DetectionTime;
    private float m_DetectionTimer = 0;

    private Vector3[] m_InvestigationPoints = new Vector3[3];
    private int m_CurrentInvestigationPointIndex = 0;

    private Vector3 m_SoundPosition;
    private bool IsInvestigatingSound = false;
    private bool IsSearchingForTarget = false;

    private NavMeshPath m_NavMeshPath;
    private PatrolPathNode currentPathNode;

    private GameObject potentionalTarget;
    private Transform pursueTarget;
    private Vector3 seekTarget;
    private Timer m_Timer;

    [SerializeField] private float m_SideMovementSpeed = 2f;
    [SerializeField] private float m_MaxSideMovementChangeInterval = 3f;
    private float m_SideMovementChangeInterval;
    private float m_SideMovementTimer = 0f;
    private bool m_IsMovingRight = true;
    // Unity Event

    private void Start()
    {
        m_Timer = Timer.CreateTimer(findTargetTimer, true);
        m_Timer.OnTick += SearchForNewTarget;
        m_Timer.Play();

        m_characterMovement.UpdatePosition = false;
        m_NavMeshPath = new NavMeshPath();

        m_AlienSoldier.EventOnDeath.AddListener(OnAIDeath);
        m_AlienSoldier.OnGetDamage += OnGetDamage;

        m_SideMovementChangeInterval = Random.Range(0, m_MaxSideMovementChangeInterval);

        SoundEvent.OnSoundEmmited.AddListener(OnSoundHeard);
        StartBehaviour(m_AIBehaviour);
    }

    private void OnDestroy()
    {
        if (m_AlienSoldier != null)
            m_AlienSoldier.OnGetDamage -= OnGetDamage;

        if (m_Timer != null)
            m_Timer.OnTick -= SearchForNewTarget;

        if (m_PatrolPath != null && currentPathNode != null)
        {
            m_PatrolPath.ReleaseNode(currentPathNode.transform.position);
        }

        m_AlienSoldier.EventOnDeath.RemoveListener(OnAIDeath);
        SoundEvent.OnSoundEmmited.RemoveListener(OnSoundHeard);
    }

    private void Update()
    {
        SyncAgentAndCharacterMovement();
        UpdateAI();
    }

    // Handler

    private void OnAIDeath()
    {
        if (pursueTarget != null && IsPlayerTarget(pursueTarget.gameObject))
        {
            PlayerVisibilityManager.Instance.RegisterVisibility(false, this);
            DetectionIndicator.Instance.SetVisible(false);
        }

        StopAllCoroutines();

        if (currentPathNode != null)
        {
            m_PatrolPath.ReleaseNode(currentPathNode.transform.position);
        }

        if (m_Timer != null)
        {
            m_Timer.OnTick -= SearchForNewTarget;
        }

        SoundEvent.OnSoundEmmited.RemoveListener(OnSoundHeard);
    }

    private void OnGetDamage(DestructibleBase other)
    {
        if (other.TeamId != m_AlienSoldier.TeamId)
        {
            ActionAssingTargetAllTeamMember(other.transform);
        }
    }

    private void OnSoundHeard(Vector3 soundPosition, float soundRange)
    {
        float distanceToSound = Vector3.Distance(transform.position, soundPosition);

        if (distanceToSound <= m_HearingRange && distanceToSound <= soundRange)
        {
            m_SoundPosition = soundPosition;
            IsSearchingForTarget = true;
            GenerateInvestigationPoints(m_SoundPosition, 10f);
            m_CurrentInvestigationPointIndex = 0;
            StartBehaviour(AIBehaviour.SeekTarget);
        }
    }

    //AI

    private void UpdateAI()
    {
        ActionUpdateTarget();

        if (m_AIBehaviour == AIBehaviour.Idle)
        {
            return;
        }

        if (m_AIBehaviour == AIBehaviour.PursuetTarget)
        {
            if (pursueTarget == null) return;

            float distanceToTarget = Vector3.Distance(transform.position, pursueTarget.position);

            if (distanceToTarget < m_MinDistanceToTarget / 2)
            {
                MoveAwayFromTarget();
                return;
            }

            if (distanceToTarget > m_MinDistanceToTarget)
            {
                m_Agent.CalculatePath(pursueTarget.position, m_NavMeshPath);
                m_Agent.SetPath(m_NavMeshPath);
            }

            if (distanceToTarget <= m_AimingDistance)
            {
                m_Agent.CalculatePath(ActionMoveSideways(), m_NavMeshPath);
                m_Agent.SetPath(m_NavMeshPath);

                Vector3 directionToTarget = (pursueTarget.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

                    m_characterMovement.Aiming();

                if (pursueTarget.transform.root.GetComponent<CharacterController>() == true)
                    m_AlienSoldier.Fire(pursueTarget.position + new Vector3(0, 1, 0));
                else
                    m_AlienSoldier.Fire(pursueTarget.position);
            }
            else
            {
                    m_characterMovement.UnAiming();
            }

            if (pursueTarget != null && pursueTarget.transform.root.GetComponent<Destructible>().IsDead == true)
            {
                if (pursueTarget != null && IsPlayerTarget(pursueTarget.gameObject))
                {
                    PlayerVisibilityManager.Instance.RegisterVisibility(false, this);
                    DetectionIndicator.Instance.SetVisible(false);
                }

                potentionalTarget = null;
                pursueTarget = null;
                m_characterMovement.UnAiming();

                StartBehaviour(AIBehaviour.PatrolRandom);
            }
        }

        if (m_AIBehaviour == AIBehaviour.SeekTarget)
        {
            if (pursueTarget == null && IsSearchingForTarget)
            {
                if (m_CurrentInvestigationPointIndex < m_InvestigationPoints.Length)
                {
                    m_Agent.CalculatePath(m_InvestigationPoints[m_CurrentInvestigationPointIndex], m_NavMeshPath);
                    m_Agent.SetPath(m_NavMeshPath);
                    m_characterMovement.Run();

                    if (AgentReachedDestination())
                    {
                        m_CurrentInvestigationPointIndex++;

                        if (m_CurrentInvestigationPointIndex >= m_InvestigationPoints.Length)
                        {
                            // Все точки пройдены, выходим из состояния поиска
                            m_characterMovement.UnRun();
                            IsSearchingForTarget = false;
                            StartBehaviour(AIBehaviour.PatrolRandom);
                        }
                    }
                }
            }
        }

        if (m_AIBehaviour == AIBehaviour.PatrolRandom || m_AIBehaviour == AIBehaviour.PatrolCircle)
        {
            m_characterMovement.UnRun();
            m_characterMovement.UnAiming();

            if (AgentReachedDestination() && currentPathNode != null)
            {
                // Освобождаем текущую точку
                m_PatrolPath.ReleaseNode(currentPathNode.transform.position);

                StartCoroutine(SetBehaviourOnTime(AIBehaviour.Idle, currentPathNode.Idle_Time));
            }
        }
    }

    // Actions

    private void ActionUpdateTarget()
    {
        if (potentionalTarget == null) return;


        if (potentionalTarget.transform.root.GetComponent<Destructible>().IsDead == true)
        {
            potentionalTarget = null;
            return;
        }

        if (m_ColliderViewer.IsObjectVisible(potentionalTarget))
        {
            m_DetectionTimer = 0;

            if (IsPlayerTarget(potentionalTarget))
            {
                PlayerVisibilityManager.Instance.RegisterVisibility(true, this);
            }

            if (pursueTarget != potentionalTarget)
            {
                pursueTarget = potentionalTarget.transform;
                ActionAssingTargetAllTeamMember(pursueTarget);
            }
        }
        else if (m_ColliderViewer.IsObjectVisibleInSideView(potentionalTarget))
        {
            m_DetectionTimer += Time.deltaTime;

            if (m_DetectionTimer >= m_DetectionTime)
            {
                if (IsPlayerTarget(potentionalTarget))
                {
                    PlayerVisibilityManager.Instance.RegisterVisibility(true, this);
                }

                if (pursueTarget != potentionalTarget)
                {
                    pursueTarget = potentionalTarget.transform;
                    ActionAssingTargetAllTeamMember(pursueTarget);
                }
            }
        }
        else
        {
            m_DetectionTimer = 0;

            if (pursueTarget != null)
            {
                if (IsPlayerTarget(pursueTarget.gameObject))
                {
                    PlayerVisibilityManager.Instance.RegisterVisibility(false, this);
                    DetectionIndicator.Instance.SetVisible(false);
                }

                seekTarget = pursueTarget.position;
                pursueTarget = null;

                IsSearchingForTarget = true;
                IsInvestigatingSound = false;

                GenerateInvestigationPoints(seekTarget, 10f);
                m_CurrentInvestigationPointIndex = 0;
                StartBehaviour(AIBehaviour.SeekTarget);
            }
        }
    }

    private Vector3 ActionMoveSideways()
    {
        if (pursueTarget == null)
        {
            Debug.LogWarning("No pursue target!");
            return transform.position; // Возвращаем текущую позицию, если цель отсутствует
        }

        m_SideMovementTimer += Time.deltaTime;
        if (m_SideMovementTimer >= m_SideMovementChangeInterval)
        {
            m_SideMovementChangeInterval = Random.Range(0, m_MaxSideMovementChangeInterval);
            m_SideMovementTimer = 0f;
            m_IsMovingRight = !m_IsMovingRight;
        }

        // Направление к цели
        Vector3 directionToTarget = (pursueTarget.position - transform.position).normalized;

        // Перпендикулярное направление (влево или вправо относительно направления к цели)
        Vector3 perpendicularDirection = m_IsMovingRight ? Vector3.Cross(directionToTarget, Vector3.up) : Vector3.Cross(Vector3.up, directionToTarget);

        // Точка для движения
        Vector3 targetPosition = transform.position + perpendicularDirection * m_SideMovementSpeed;

        return targetPosition;
    }

    private void MoveAwayFromTarget()
    {
        if (pursueTarget == null) return;

        Vector3 directionAwayFromTarget = (transform.position - pursueTarget.position).normalized;

        Vector3 targetPosition = transform.position + directionAwayFromTarget * m_SideMovementSpeed;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            m_Agent.CalculatePath(hit.position, m_NavMeshPath);
            m_Agent.SetPath(m_NavMeshPath);
        }

        Vector3 directionToTarget = (pursueTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        m_characterMovement.Aiming();
    }

    private void ActionAssingTargetAllTeamMember(Transform other)
    {
        if (other.transform.root.GetComponent<Destructible>().IsDead == true) return;

        List<Destructible> team = Destructible.GetAllTeamMembers(m_AlienSoldier.TeamId);

        foreach (Destructible dest in team)
        {
            AIAlienSoldier ai = dest.transform.root.GetComponent<AIAlienSoldier>();

            if (ai != null && ai.enabled == true)
            {
                ai.SetPersueTarget(other);
                ai.StartBehaviour(AIBehaviour.PursuetTarget);
            }
        }
    }

    // Behaviour

    private void StartBehaviour(AIBehaviour state)
    {
        if (m_AlienSoldier.IsDead == true) return;

        if (state != AIBehaviour.PatrolRandom && state != AIBehaviour.PatrolCircle && currentPathNode != null)
        {
            m_PatrolPath.ReleaseNode(currentPathNode.transform.position);
        }

        if (state == AIBehaviour.Idle)
        {
            m_characterMovement.ResetState();

            m_Agent.isStopped = true;
        }

        if (state == AIBehaviour.PatrolCircle)
        {
            m_characterMovement.ResetState();

            m_Agent.isStopped = false;

            SetDestinationByPathNode(m_PatrolPath.GetNextPathNode(ref m_PathNodeIndex));
        }

        if (state == AIBehaviour.PatrolRandom)
        {
            m_characterMovement.ResetState();

            m_Agent.isStopped = false;

            SetDestinationByPathNode(m_PatrolPath.GetRandomPathNode());
        }

        if (state == AIBehaviour.PursuetTarget)
        {
            m_Agent.isStopped = false;
        }

        if (state == AIBehaviour.SeekTarget)
        {
            m_Agent.isStopped = false;
            m_characterMovement.ResetState();
        }

        m_AIBehaviour = state;
    }

    IEnumerator SetBehaviourOnTime(AIBehaviour state, float second)
    {
        if (m_AIBehaviour == AIBehaviour.SeekTarget)
        {
            yield break; // Если текущее состояние — SeekTarget, выходим из корутины
        }

        AIBehaviour previous = m_AIBehaviour;
        m_AIBehaviour = state;
        StartBehaviour(m_AIBehaviour);

        yield return new WaitForSeconds(second);

        if (!IsInvestigatingSound && !IsSearchingForTarget)
            StartBehaviour(previous);
    }

    // Private Methods

    private void SyncAgentAndCharacterMovement()
    {
        m_Agent.speed = m_characterMovement.CurrentSpeed;

        float factor = m_Agent.velocity.magnitude / m_Agent.speed;
        m_characterMovement.TargetDirectionControl = transform.InverseTransformDirection(m_Agent.velocity.normalized) * factor;
    }

    private void SetDestinationByPathNode(PatrolPathNode node)
    {
        if (node == null) return;

        if (currentPathNode != null)
            m_PatrolPath.ReleaseNode(currentPathNode.transform.position);

        currentPathNode = node;
        m_Agent.CalculatePath(node.transform.position, m_NavMeshPath);
        m_Agent.SetPath(m_NavMeshPath);
    }

    private bool AgentReachedDestination()
    {
        if (!m_Agent.pathPending && m_Agent.remainingDistance <= m_Agent.stoppingDistance)
        {
            if (!m_Agent.hasPath || m_Agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }
        return false;
    }

    private void SearchForNewTarget()
    {
        GameObject target = Destructible.FindNearestNonTeamMember(m_AlienSoldier)?.gameObject;

        if (target != null && target.transform.root.GetComponent<Destructible>().IsDead == false)
        {
            potentionalTarget = target;
            m_Timer.Restart();
        }
        else
        {
            potentionalTarget = null;
        }
    }

    private void GenerateInvestigationPoints(Vector3 center, float radius)
    {
        for (int i = 0; i < m_InvestigationPoints.Length; i++)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitSphere * radius;
            m_InvestigationPoints[i] = center + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
    }

    private bool IsPlayerTarget(GameObject target)
    {
        return target.CompareTag("Player");
    }

    // Pablic Method
    public void SetPersueTarget(Transform target)
    {
        pursueTarget = target;
    }
}
