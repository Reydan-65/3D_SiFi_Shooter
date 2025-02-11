using UnityEngine;

public class ColliderViewer : MonoBehaviour
{
    [SerializeField] private float m_ViewingAngle;
    [SerializeField] private float m_ViewingDistance;
    [SerializeField] private float m_ViewingHeight;
    [SerializeField] private float m_AdditionalAngle; // Новый угол
    [SerializeField] private float m_AdditionalViewingDistance;

    // Public API
    public bool IsObjectVisible(GameObject target)
    {
        ColliderViewPoint viewPoint = target.GetComponent<ColliderViewPoint>();
        if (viewPoint == null) return false;

        Vector3 viewPosition = transform.position + new Vector3(0, m_ViewingHeight, 0);
        Vector3 directionToTarget = target.transform.position - viewPosition;

        float angle = Vector3.Angle(transform.forward, directionToTarget);

        if (angle <= m_ViewingAngle)
        {
            bool isVisible = viewPoint.IsVisibleFromPoint(viewPosition, transform.forward, m_ViewingAngle, m_ViewingDistance);
            return isVisible;
        }

        return false;
    }

    public bool IsObjectVisibleInSideView(GameObject target)
    {
        ColliderViewPoint viewPoint = target.GetComponent<ColliderViewPoint>();
        if (viewPoint == null) return false;

        Vector3 viewPosition = transform.position + new Vector3(0, m_ViewingHeight, 0);
        Vector3 directionToTarget = target.transform.position - viewPosition;

        Quaternion additionalRotation1 = Quaternion.Euler(0, -60, 0);
        Vector3 additionalForward1 = transform.rotation * additionalRotation1 * Vector3.forward;

        float angle1 = Vector3.Angle(additionalForward1, directionToTarget);
        bool isVisibleInFrustum1 = angle1 <= m_AdditionalAngle && viewPoint.IsVisibleFromPoint(viewPosition, additionalForward1, m_AdditionalAngle, m_AdditionalViewingDistance);

        Quaternion additionalRotation2 = Quaternion.Euler(0, 60, 0);
        Vector3 additionalForward2 = transform.rotation * additionalRotation2 * Vector3.forward;

        float angle2 = Vector3.Angle(additionalForward2, directionToTarget);
        bool isVisibleInFrustum2 = angle2 <= m_AdditionalAngle && viewPoint.IsVisibleFromPoint(viewPosition, additionalForward2, m_AdditionalAngle, m_AdditionalViewingDistance);

        return isVisibleInFrustum1 || isVisibleInFrustum2;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + new Vector3(0, m_ViewingHeight, 0),
                                      transform.rotation, Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, m_ViewingAngle, 0, m_ViewingDistance, 1);

        Gizmos.color = Color.red;
        Quaternion additionalRotation1 = Quaternion.Euler(0, -60, 0);
        Gizmos.matrix = Matrix4x4.TRS(transform.position + new Vector3(0, m_ViewingHeight, 0),
                                      transform.rotation * additionalRotation1, Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, m_AdditionalAngle, 0, m_AdditionalViewingDistance, 1);

        Quaternion additionalRotation2 = Quaternion.Euler(0, 60, 0);
        Gizmos.matrix = Matrix4x4.TRS(transform.position + new Vector3(0, m_ViewingHeight, 0),
                                      transform.rotation * additionalRotation2, Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, m_AdditionalAngle, 0, m_AdditionalViewingDistance, 1);
    }
#endif
}
