using Unity.Burst.CompilerServices;
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

        if (Physics.Raycast(ray, out RaycastHit hit, 1000, ~ignoreTriggerLayerMask))
        {
            weapon.FirePointLookAt(hit.point);
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
        Gizmos.color = Color.yellow;
        Ray ray = mainCamera.ScreenPointToRay(imageSight.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000, ~ignoreTriggerLayerMask))
        {
            Gizmos.DrawLine(transform.position, hit.point);
        }
    }

#endif


}
