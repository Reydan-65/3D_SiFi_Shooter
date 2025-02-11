using UnityEngine;

public class VehicleShooter : MonoBehaviour
{
    [SerializeField] private VehicleTurret m_Turret;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform imageSight;
    [SerializeField] private LayerMask ignoreTriggerLayerMask;

    public void Shoot()
    {
        Ray ray = mainCamera.ScreenPointToRay(imageSight.position);

        Vector3 rayOrigin = m_Turret.FirePoint.position;
        Vector3 rayDir = ray.direction;

        Ray weaponRay = new Ray(rayOrigin, rayDir);

        if (Physics.Raycast(weaponRay, out RaycastHit hit, 1000, ~ignoreTriggerLayerMask))
        {
            m_Turret.FirePointLookAt(hit.point);
        }

        if (m_Turret.CanFire == true)
        {
            m_Turret.Fire();
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (m_Turret.FirePoint == null || mainCamera == null || imageSight == null)
            return;

        Gizmos.color = Color.yellow;
        Ray ray = mainCamera.ScreenPointToRay(imageSight.position);
        Vector3 rayOrigin = m_Turret.FirePoint.position;
        Vector3 rayDirection = ray.direction;
        Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * 1000);
    }

#endif

}
