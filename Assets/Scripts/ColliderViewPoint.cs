using UnityEngine;

public class ColliderViewPoint : MonoBehaviour
{
    private enum ColliderType
    {
        None,
        Character,
        Drone
    }

    [SerializeField] private ColliderType m_ColliderType;

    [SerializeField] private Collider m_Collider;

    private Vector3[] m_Points;

    private void Start()
    {
        UpdateViewPoints();
    }

    private void Update()
    {
        if (m_ColliderType == ColliderType.Character)
        {
            CalculatePointForCharacterController(m_Collider as CharacterController);
        }
        else if (m_ColliderType == ColliderType.Drone)
        {
            CalculatePointsForDrone(m_Collider as MeshCollider);
        }
    }

    // Public API

    public bool IsVisibleFromPoint(Vector3 point, Vector3 eyeDir, float viewAngle, float viewDistance)
    {
        for (int i = 0; i < m_Points.Length; i++)
        {
            float angle = Vector3.Angle(m_Points[i] - point, eyeDir);
            float distance = Vector3.Distance(m_Points[i], point);

            if (angle <= viewAngle * 0.5f && distance <= viewDistance)
            {
                RaycastHit hit;

                Debug.DrawLine(point, m_Points[i], Color.blue);
                if (Physics.Raycast(point, (m_Points[i] - point).normalized, out hit, viewDistance * 2) == true)
                {
                    if (hit.collider == m_Collider)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // Private API
    [ContextMenu("Update View Points")]
    private void UpdateViewPoints()
    {
        if (m_Collider == null) return;
        m_Points = null;
        if (m_ColliderType == ColliderType.Character)
        {
            UpdatePointsForCharacterControllers();
        }
        else if (m_ColliderType == ColliderType.Drone)
        {
            UpdatePointsForDrone();
        }
    }

    private void UpdatePointsForCharacterControllers()
    {
        if (m_Points == null)
        {
            m_Points = new Vector3[4];
        }

        CharacterController collider = m_Collider as CharacterController;

        CalculatePointForCharacterController(collider);
    }

    private void UpdatePointsForDrone()
    {
        if (m_Points == null)
        {
            m_Points = new Vector3[5]; // 4 угла + центр
        }

        MeshCollider collider = m_Collider as MeshCollider;

        CalculatePointsForDrone(collider);
    }

    private void CalculatePointForCharacterController(CharacterController collider)
    {
        m_Points[0] = collider.transform.position + collider.center + collider.transform.up * collider.height * 0.3f; ;
        m_Points[1] = collider.transform.position + collider.center - collider.transform.up * collider.height * 0.3f; ;
        m_Points[2] = collider.transform.position + collider.center + collider.transform.right * collider.radius * 0.4f; ;
        m_Points[3] = collider.transform.position + collider.center - collider.transform.right * collider.radius * 0.4f; ;
    }

    private void CalculatePointsForDrone(MeshCollider meshCollider)
    {
        m_Points[0] = meshCollider.transform.position + meshCollider.transform.up * 0.5f;
        m_Points[1] = meshCollider.transform.position - meshCollider.transform.up * 0.4f;
        m_Points[2] = meshCollider.transform.position + meshCollider.transform.right * 0.4f;
        m_Points[3] = meshCollider.transform.position - meshCollider.transform.right * 0.4f;
        m_Points[4] = meshCollider.transform.position + meshCollider.transform.forward * 1f + meshCollider.transform.up * 0.5f;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (m_Points == null) return;

        Gizmos.color = Color.blue;
        for (int i = 0; i < m_Points.Length; i++)
            Gizmos.DrawSphere(m_Points[i], 0.1f);
    }

#endif

}
