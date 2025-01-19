using UnityEngine;
using UnityEngine.UI;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private Weapon weapon;
    [SerializeField] private SpreadShootRig spreadShootRig;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform imageSight;

    public void Shoot()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(imageSight.position);

        if (Physics.Raycast(ray, out hit, 1000))
        {
            weapon.FirePointLookAt(hit.point);
        }

        if (weapon.CanFire == true)
        {
            weapon.Fire();
            spreadShootRig.Spread();
        }
    }
}
