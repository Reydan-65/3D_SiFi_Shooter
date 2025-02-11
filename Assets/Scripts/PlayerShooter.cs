using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private Weapon weapon;
    [SerializeField] private SpreadShootRig spreadShootRig;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform imageSight;
    [SerializeField] private LayerMask ignoreTriggerLayerMask;

    public void Shoot()
    {
        Ray ray = mainCamera.ScreenPointToRay(imageSight.position);

        Vector3 rayOrigin = mainCamera.transform.position;
        Vector3 rayDir = ray.direction;

        Ray weaponRay = new Ray(rayOrigin, rayDir);

        RaycastHit[] hits = Physics.RaycastAll(weaponRay, 1000);

        foreach (RaycastHit hit in hits)
        {
            // Игнорируем триггеры
            if (!hit.collider.isTrigger)
            {
                weapon.FirePointLookAt(hit.point);
                break;
            }
        }

        if (weapon.CanFire == true)
        {
            weapon.Fire();
            spreadShootRig.Spread();
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (mainCamera == null || mainCamera == null || imageSight == null)
            return;

        Gizmos.color = Color.yellow;
        Ray ray = mainCamera.ScreenPointToRay(imageSight.position);
        Vector3 rayOrigin = mainCamera.transform.position;
        Vector3 rayDirection = ray.direction;
        Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * 1000);
    }

#endif

}
