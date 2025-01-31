using UnityEngine;

public class AimPoint : MonoBehaviour
{
    [SerializeField] private Weapon targetWeapon;

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000) == true)
        {
            if (!hit.collider.isTrigger)
                transform.position = hit.point;
        }
    }
}
